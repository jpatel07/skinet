using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;

        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // get basket from the repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // get item from the product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrderd = new ProducItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrderd, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            // get delivery method
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // calculate subtotal
            var subTotal = items.Sum(item => item.Price * item.Quantity);

            // check to see if order exists 
            var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntendId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntendId);
            }


            // create order
            var order = new Order(items,
                                  buyerEmail,
                                  shippingAddress,
                                  deliveryMethod,
                                  subTotal,
                                  basket.PaymentIntendId);

            _unitOfWork.Repository<Order>().Add(order);

            // save to db
            var result = await _unitOfWork.Complete();

            if (result <= 0) return null;

             return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndORderingSpecification(id, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndORderingSpecification(buyerEmail);
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}