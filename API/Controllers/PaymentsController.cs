using System.IO;
using System.Threading.Tasks;
using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private const string WhSecret = "whsec_v41LgBDgu8VuWt57UXB1WiayGAlJXKMI";
        private readonly ILogger<IPaymentService> _logger;
        public PaymentsController(IPaymentService paymentService, ILogger<IPaymentService> logger)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));
            return basket;
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebHook()
        {
             _logger.LogInformation("-------------StripeWebHook-------------");
             _logger.LogInformation("--------------StripeWebHook------------");
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
            WhSecret, throwOnApiVersionMismatch: false);

            PaymentIntent intent;
            Order order;
              _logger.LogInformation("--------------StripeWebHook------------ ", stripEvent.Type);
            switch (stripEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripEvent.Data.Object;
                    _logger.LogInformation("Payment Succedded: ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order Updated to Payment Received: ", order.Id);
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");

                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripEvent.Data.Object;
                    _logger.LogInformation("Payment failed: ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    _logger.LogInformation("Order Updated to Payment Failed: ", order.Id);
                    _logger.LogError("Order Updated to Payment Failed: ", order.Id);
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    _logger.LogInformation("--------------------------");
                    break;
            }

            return new EmptyResult();
        }
    }
}