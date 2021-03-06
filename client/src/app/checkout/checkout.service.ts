import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';
import { map } from 'rxjs/operators';
import { IOrderToCreate, IOrder } from '../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createOrder(order: IOrderToCreate) {
    return this.http.post(this.baseUrl + 'orders', order);

  }
  getDeliveryMethods() {
    return this.http.get(this.baseUrl + 'orders/deliveryMethods').pipe(
      map((dm: IDeliveryMethod[]) => {
        return dm.sort((a, b) => b.price - a.price);
      })
    );
  }

  getUserOrders() {
    return this.http.get(this.baseUrl + 'orders').pipe(
      map((orders: IOrder[]) => {
              return orders;
      })
    );
  }

  getUserOrder(id: number) {
    return this.http.get(this.baseUrl + 'orders/' + id).pipe(
      map((order: IOrder) => {
        return order;
      })
    );
  }
}
