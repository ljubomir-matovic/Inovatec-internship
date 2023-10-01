import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';

@Component({
  selector: 'app-accept-orders',
  templateUrl: './accept-orders.component.html',
  styleUrls: ['./accept-orders.component.scss']
})
export class AcceptOrdersComponent implements OnInit {

  constructor(private orderService: OrderService, private router: Router){}

  loading: boolean = true;
  orderId: string = "";
  isError: boolean = false;
  errorMessage: string = "";

  ngOnInit(): void {
    this.orderService.acceptOrders().subscribe({
      next: (actionResult: ActionResultResponse<string>) => {
        this.loading = false;
        if(actionResult.actionSuccess) {
          this.orderId = actionResult.actionData;
        } else {
          this.isError = true;

          if(actionResult.actionData == "EmptyCart") {
            this.errorMessage = "OrderAdministration.EmptyCart";
          } else {
            this.errorMessage = "OrderAdministration.OrderFailed";
          }
        }
      },
      error: () => {
        this.loading = false;
        this.isError = true;
        this.errorMessage = "OrderAdministration.OrderFailed";
      }
    });
  }
}
