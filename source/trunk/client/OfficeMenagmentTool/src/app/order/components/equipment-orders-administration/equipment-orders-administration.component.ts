import { ChangeDetectorRef, Component, EventEmitter, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, FilterMatchMode,  LazyLoadEvent, MessageService, SelectItem } from 'primeng/api';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ORDER_STATES, OrderState } from 'src/app/shared/constants/order-states';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Order } from 'src/app/shared/models/order';
import { OrderFilter } from 'src/app/shared/models/order-filter';
import { OpenEquipmentOrderComponent } from '../open-equipment-order/open-equipment-order.component';
import { ROLES, Role } from 'src/app/shared/constants/role';
import { OrderRequestService } from '../../services/order-request.service';

@Component({
  selector: 'app-equipment-orders-administration',
  templateUrl: './equipment-orders-administration.component.html',
  styleUrls: ['./equipment-orders-administration.component.scss']
})
export class EquipmentOrdersAdministrationComponent {
  @ViewChild("equipmentTable", { static: true }) equipmentTable!: Table;
  orders: Order[] = [];
  lastLoadEvent!: LazyLoadEvent;
  loading: boolean = true;

  forCurrentUserOnly: boolean = false;

  UserRole = Role;

  orderState = OrderState;
  orderStates: any = ORDER_STATES;

  selectedStates: number[] = [this.orderStates[OrderState.Pending], this.orderStates[OrderState.InProgress]];

  pageSize: number = 10;
  totalRecords: number = 1;
  first: number = 0;

  userRoles = ROLES;

  subscriptions: EventEmitter<any>[] = [];
  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();
  dialogRef!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;

  get matchModelOptions(): SelectItem[] {
    return [{ label: "Contains", value: FilterMatchMode.CONTAINS }];
  }

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  get toastKey():string {
    return "toast";
  }

  constructor(
    public storageService: StorageService,
    private orderRequestService: OrderRequestService,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private dialogService: DialogService,
    private route: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initSubscriptions();

    this.dialogRef = new DynamicDialogRef();
    this.dialogStatus = new EventEmitter<any>();

    this.forCurrentUserOnly = this.route.snapshot.data["forCurrentUserOnly"];
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  initSubscriptions(): void {
    this.subscriptions.push(this.onDialogClose);

    this.onDialogClose.subscribe({
      next: (status: ActionResult<any>) => {
        this.dialogClosed(status);
      }
    });
  }

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  lazyLoadEquipmentOrders(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let orderFilter: OrderFilter = {
      Name: '',
      Item: '',
      Description: '',
      PageNumber: this.getPageNumber(event.first, this.pageSize),
      PageSize: this.pageSize,
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      orderFilter.Name = event.filters["name"]?.value ?? null;
      orderFilter.Description = event.filters["description"]?.value ?? null;
    }

    orderFilter.States = this.selectedStates.map((x: any) => x.id) ?? null;

    if(this.forCurrentUserOnly) {
      orderFilter.Users = [this.storageService.getUserData()!.id];
    }

    this.orders = [];
    this.orderRequestService.getEquipmentOrder(orderFilter).subscribe({
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

  refreshOrders(action: CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.lazyLoadEquipmentOrders({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        break;

      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.pageSize);

        if((pageNumber - 1) * this.pageSize == this.totalRecords - 1 && pageNumber > 1){
          this.lazyLoadEquipmentOrders({ ...this.lastLoadEvent, first: first-this.pageSize, rows: this.pageSize });
        }
        else {
          this.lazyLoadEquipmentOrders({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        }
        break;
    }
  }

  filterByStates(table: Table): void {
    table.filter(this.selectedStates, "states", this.matchMode);
  }

  filterOrders(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  showDeleteConfirm(id: number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.confirmationService.confirm({
        message: translations['OrderAdministration']['DeleteEquipmentOrderConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.orderRequestService.deleteOrder(id).subscribe({
            next:(response: any) => {
              this.refreshOrders(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['OrderAdministration'][response.actionData], key: this.toastKey });
            }
          });
        },
        reject: () => {
          this.confirmationService.close();
        }
      });
    });
  }

  openEquipmentOrder(event: any): void {
    let order: Order = event.data;
    this.dialogRef = this.dialogService.open(
      OpenEquipmentOrderComponent,
      {
        maximizable: false,
        dismissableMask: true,
        data: {
          order,
          formSubmitted: this.onDialogClose
        }
      }
    );
  }

  dialogClosed(status: any): void {
    switch (status.data.action) {
      case CRUDActions.Update:
        let receivedOrder: Order = status.data.order;
        let orderIndex: number = this.orders.findIndex(order => order.id == receivedOrder.id);
        if(orderIndex != -1) {
          this.orders[orderIndex] = receivedOrder;
          this.changeDetectorRef.detectChanges();
        }
        break;

      case CRUDActions.Delete:
        this.refreshOrders(CRUDActions.Delete);
        this.dialogRef.close();
        break;

      default:
        break;
    }
  }
}
