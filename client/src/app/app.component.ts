import { Component, OnInit } from '@angular/core';
import { BasketService } from './basket/basket.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  title = 'Skinet';
  constructor(private basketService: BasketService) {}
  ngOnInit(): void {
    const baskitId = localStorage.getItem('basket_id');
    if(baskitId) {
      this.basketService.getBasket(baskitId).subscribe(() =>
      {
        console.log('intialised basket');
      }, error => {
        console.log(error);
      });
    }


   }
}
