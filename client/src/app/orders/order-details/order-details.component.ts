import { Component, OnInit } from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { ActivatedRoute } from '@angular/router';
import { CheckoutService } from 'src/app/checkout/checkout.service';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss']
})
export class OrderDetailsComponent implements OnInit {
  placedOrder: IOrder;

  constructor(private checkoutService: CheckoutService, private activateRoute: ActivatedRoute,
    private bcService: BreadcrumbService) {
    this.bcService.set('@OrderDetails', '');
  }

  ngOnInit(): void {
    this.loadOrder();
  }
  loadOrder() {

    this.checkoutService.getUserOrder(+this.activateRoute.snapshot.paramMap.get('id')).subscribe(order => {
      this.placedOrder = order;
    });
  }

}
