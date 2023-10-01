import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { ORDER_STATES, OrderState } from 'src/app/shared/constants/order-states';
import { SignalRService } from 'src/app/shared/services/signalR.service';

@Component({
  selector: 'app-single-order',
  templateUrl: './single-order.component.html',
  styleUrls: ['./single-order.component.scss']
})
export class SingleOrderComponent implements OnInit, OnDestroy {
  order: any;
  orderId: number = 0;
  loadingPage: boolean = true;

  readonly orderStates = ORDER_STATES;

  attachments: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService,
    private signalRService: SignalRService
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(data => {
      this.orderId = Number(data["id"]);

      if(isNaN(this.orderId)) {
        this.router.navigateByUrl("/orders");
      }

      this.orderService.getOrderById(this.orderId).subscribe({
        next: (order: any) => {
          if(order == null) {
            this.router.navigateByUrl("/orders");
          }

          this.order = order;
          this.order.state = this.orderStates[this.order.state];
          this.loadingPage = false;
        },
        error: (err: any) => {
          this.loadingPage = false;
          this.router.navigateByUrl("/orders");
        }
      });

      this.signalRService.reopen()
      .then((connection) => {
        connection.on("changedStateOfOrder" + this.orderId, (state: OrderState) => {
          this.order.state = this.orderStates[state];
        });
      });
    });
  }

  ngOnDestroy(): void {
    let connection = this.signalRService.instance;
    if(this.signalRService.canRemoveListeners) {
      connection.off("changedStateOfOrder" + this.orderId);
    }
  }

  changeStateOnServer(): void {
    this.orderService.changeOrderState(this.orderId, this.order.state.id).subscribe({
      next: () => {

      }
    })
  }
}
