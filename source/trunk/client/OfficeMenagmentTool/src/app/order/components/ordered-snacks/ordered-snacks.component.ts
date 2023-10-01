import { ChangeDetectorRef, Component, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, FilterMatchMode, LazyLoadEvent, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { Observable } from 'rxjs';
import { OrderRequestService } from 'src/app/order/services/order-request.service';
import { GROUP_BY_OPTIONS, GroupByOption } from 'src/app/shared/constants/group-by-options';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Order } from 'src/app/shared/models/order';
import { OrderFilter } from 'src/app/shared/models/order-filter';
import { Role } from 'src/app/shared/constants/role';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { Office } from 'src/app/shared/models/office';
import { OfficeService } from 'src/app/office/services/office.service';
import { UpdateSnackOrderComponent } from '../update-snack-order/update-snack-order.component';
import { ActionResult } from 'src/app/shared/models/action-result.model';

@Component({
  selector: 'app-ordered-snacks',
  templateUrl: './ordered-snacks.component.html',
  styleUrls: ['./ordered-snacks.component.scss']
})
export class OrderedSnacksComponent {
  orderedSnacks: Order[] = [];
  lazyLoadEvent!: LazyLoadEvent;
  loading: boolean = true;

  subscriptions: EventEmitter<any>[] = [];
  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();
  dynamicDialogRef!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;

  pageSize: number = 10;
  totalRecords: number = 1;
  first: number = 0;

  tableLoadActions = CRUDActions;
  GroupByOption = GroupByOption;
  groupByOptions = GROUP_BY_OPTIONS;
  selectedGroupByOption: number = GroupByOption.None;

  offices!: Office[];

  UserRole = Role;


  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  get toastKey(): string {
    return "toast";
  }

  constructor(
    private orderRequestService: OrderRequestService,
    private officeService: OfficeService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private translateService: TranslateService,
    private changeDetectorRef: ChangeDetectorRef,
    private dialogService: DialogService,
    public storageService: StorageService
  ) {}

  ngOnInit(): void {
    this.dynamicDialogRef = new DynamicDialogRef();
    this.dialogStatus = new EventEmitter<any>();

    this.getOffices();
    this.initSubscriptions();
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

  dialogClosed(status: any): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      if(status.success) {
        let receivedOrder: Order = status.data.order;
        let orderIndex: number = this.orderedSnacks.findIndex(order => order.id == receivedOrder.id);

        if(orderIndex != -1) {
          this.orderedSnacks[orderIndex] = receivedOrder;
          this.changeDetectorRef.detectChanges();
        }
        
        this.dynamicDialogRef.close();
        this.messageService.add({ severity: "success", summary: translations['Common']["Success"], detail: translations['CategoryAdministration'][status.data.message], key: this.toastKey });
      }
      else {
        this.messageService.add({ severity: "error", summary: translations['Common']["Error"], detail: translations['CategoryAdministration'][status.data.message], key: this.toastKey });
      }
    });
  }

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  getOffices(): void {
    let officeFilter: OfficeFilter = {
      PageNumber: 0,
      PageSize: 0,
      SortField: 'name',
      SortOrder: 1
    };

    this.officeService.getOffices(officeFilter).subscribe(
      {
        next: (result: ActionResultResponse<DataPage<Office>>) => {
          if(!result.actionSuccess || result.actionData == null) {
            this.offices = [];
          }
          this.offices = result.actionData.data;
          this.changeDetectorRef.detectChanges();
        },
        error: (error: any) => {
          this.offices = [];
        }
      }
    );
  }

  loadOrderedSnacks(event: LazyLoadEvent): void {
    this.lazyLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let orderFilter: OrderFilter = {
      PageNumber: this.getPageNumber(event.first, this.pageSize),
      PageSize: this.pageSize,
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      orderFilter.Name = event.filters["name"]?.value ?? null;
      orderFilter.Item = event.filters["item"]?.value ?? null;
      
      if(event.filters["offices"] !== undefined && event.filters["offices"]?.value) {
        orderFilter.Offices = event.filters["offices"]?.value.map((x: any) => x.id) ?? null
      }
    }

    let orderRequest: Observable<ActionResultResponse<DataPage<Order>>>;

    this.orderedSnacks = [];
    switch (this.selectedGroupByOption) {
      case GroupByOption.None:
        orderRequest = this.orderRequestService.getSnackOrders(orderFilter);
        break;

      case GroupByOption.Snack:
        orderRequest = this.orderRequestService.getSnackOrdersGroupedByItem(orderFilter);
        break;

      default:
        this.loading = false;
        this.first = 0;
        return;
    }

    orderRequest!.subscribe({
      next: (response: ActionResultResponse<DataPage<Order>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loading = false;
          this.orderedSnacks = [];
          this.first = 0;
        }

        this.totalRecords = response.actionData.totalRecords;
        this.loading = false;
        this.orderedSnacks = response.actionData.data;
      },
      error: (error: any) => {
        this.loading = false;
        this.orderedSnacks = [];
        this.first = 0;
      }
    });
  }

  refreshOrderedSnacks(action: CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.loadOrderedSnacks({ ...this.lazyLoadEvent, first: this.first, rows: this.pageSize });
        break;

      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.pageSize);

        if((pageNumber - 1) * this.pageSize == this.totalRecords - 1 && pageNumber > 1){
          this.loadOrderedSnacks({ ...this.lazyLoadEvent, first: first-this.pageSize, rows: this.pageSize });
        }
        else {
          this.loadOrderedSnacks({ ...this.lazyLoadEvent, first: this.first, rows: this.pageSize });
        }
        break;
    }
  }

  filterSnackOrders(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  showDeleteConfirm(id: number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.confirmationService.confirm({
        message: translations['OrderAdministration']['DeleteOrderConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.orderRequestService.deleteSnackOrder(id).subscribe({
            next:(response: any) => {
              this.refreshOrderedSnacks(CRUDActions.Delete);
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

  showSnackOrderEdit(order: Order): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.dynamicDialogRef = this.dialogService.open(
        UpdateSnackOrderComponent,
        {
          maximizable: false,
          dismissableMask: true,
          header: translations['OrderAdministration']['UpdateSnackOrder'],
          data: {
            order,
            formSubmitted: this.onDialogClose
          }
        }
      );
    });
  }
}
