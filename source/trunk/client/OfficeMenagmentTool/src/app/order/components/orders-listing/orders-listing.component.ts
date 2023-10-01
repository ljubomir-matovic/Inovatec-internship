import { Component } from '@angular/core';
import { LazyLoadEvent } from 'primeng/api';
import { Order } from 'src/app/shared/models/order';
import { OrderFilter } from 'src/app/shared/models/order-filter';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Table } from 'primeng/table';
import { ORDER_STATES, OrderState } from 'src/app/shared/constants/order-states';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { Role } from 'src/app/shared/constants/role';
import { Router } from '@angular/router';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-orders-listing',
  templateUrl: './orders-listing.component.html',
  styleUrls: ['./orders-listing.component.scss']
})
export class OrdersListingComponent {
  orders: Order[] = [];
  lastLoadEvent!: LazyLoadEvent;
  loading: boolean = true;

  forCurrentUserOnly: boolean = false;

  UserRole = Role;

  orderStates: any = ORDER_STATES;

  pageSize: number = 10;
  totalRecords: number = 1;
  first: number = 0;

  selectedStates: any[] = [ORDER_STATES[OrderState.Pending], ORDER_STATES[OrderState.InProgress]];

  constructor(
    public storageService: StorageService,
    private orderService: OrderService,
    private router: Router
  ) {}

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  loadOrders(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let orderFilter: OrderFilter = {
      Name: '',
      Description: '',
      PageNumber: this.getPageNumber(event.first, this.pageSize),
      PageSize: this.pageSize,
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      orderFilter.Name = event.filters["name"]?.value ?? null;
      orderFilter.States = this.selectedStates?.map((x: any) => x.id) ?? null;
    }

    this.orders = [];
    this.orderService.getOrdersPage(orderFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Order>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loading = false;
          this.orders = [];
          this.first = 0;
        }

        this.totalRecords = response.actionData.totalRecords;
          this.orders = response.actionData.data;
          this.loading = false;
      },
      error: (error: any) => {
        this.loading = false;
        this.orders = [];
        this.first = 0;
      }
    });
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  openPage(id: number): void {
    this.router.navigateByUrl(`/order/${id}`);
  }

  filterByState(table: Table): void {
    table.filter(this.selectedStates,'states', "CONTAINS");
  }
}
