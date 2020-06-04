import { Component, OnInit, Input } from '@angular/core';
import { CheckoutService } from '../checkout/checkout.service';
import { FormGroup } from '@angular/forms';
import {  IOrder } from '../shared/models/order';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  @Input() placedOrderForm: FormGroup;
  placedOrders: IOrder[];

  constructor(private checkoutService: CheckoutService) { }

  ngOnInit(): void {
    this.checkoutService.getUserOrders().subscribe((orders: IOrder[]) => {
      this.placedOrders = orders;
    }, error => {
      console.log(error);
    });
  }
}
