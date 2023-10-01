import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { ConfirmationService, LazyLoadEvent, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { OrderItemService } from '../../services/order-item.service';
import { TranslateService } from '@ngx-translate/core';
import { EquipmentFilter } from 'src/app/shared/models/equipment-filter.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Table } from 'primeng/table';
import { CreateOrderItemComponent } from '../create-order-item/create-order-item.component';
import { OrderState } from 'src/app/shared/constants/order-states';

@Component({
  selector: 'app-order-items',
  templateUrl: './order-items.component.html',
  styleUrls: ['./order-items.component.scss']
})
export class OrderItemsComponent implements OnInit, OnDestroy, OnChanges {
  order: any;
  @Input() orderId: number = 0;
  @Input() orderState: any = 0;

  orderItems!: any[];
  lastLoadEvent: LazyLoadEvent = { first:0 };
  totalRecords: number = 1;
  loadingOrderItems: boolean=true;
  ref!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;
  first: number = 0;
  rows: number = 10;
  position: string = "";
  categories: any[] = [];
  items: any[] = [];
  selectedEquipments!:any[];

  get disableInput(): boolean {
    return this.orderState.id == OrderState.Done || this.orderState.id == OrderState.Canceled;
  }

  subscriptions: any[] = [];
  onDialogClose!: EventEmitter<ActionResult<any>>;

  readonly toastKey: string = "toastSingleOrder";
  attachments: any[] = [];

  constructor(
    private orderItemService: OrderItemService,
    private dialogService: DialogService,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService){}

  ngOnInit(): void {
    this.onDialogClose = new EventEmitter<ActionResult<any>>();
    this.initSubscriptions();
    this.ref = new DynamicDialogRef();
    this.setCategories();
    this.setItems();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.lastLoadEvent.first = 0;
    this.refreshOrderItems(CRUDActions.Create);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  getPageNumber(first:number,rows:number): number {
    return ( first / rows ) + 1;
  }

  initSubscriptions(): void {
    this.subscriptions.push(this.onDialogClose.subscribe({
      next: (status: any) => {
        this.dialogClosed(status);
      }
    }));
  }

  dialogClosed(status: ActionResult<any>): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{

      if(status.success) {
        this.refreshOrderItems(status.data.action);
        this.ref.close();
        this.messageService.add({ severity: "success", summary: translations['Common']["Success"], detail: translations['OrderAdministration'][status.data.message], key: this.toastKey });
      } else {
        this.messageService.add({ severity: "error", summary: translations["Common"]["Error"], detail: translations['OrderAdministration'][status.data.message], key: this.toastKey });
      }
    });
  }

  loadItems(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loadingOrderItems = true;

    if(event.first === undefined || event.rows === undefined)
      return;

    let filter:EquipmentFilter = {
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? "",
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1,
      PageSize: event.rows ?? 10,
      UserId: 0
    };
    if(event.filters !== undefined) {
      filter.ItemId = event.filters["itemId"]?.value ?? 0;
      filter.CategoryId = event.filters["categoryId"]?.value ?? 0;
    }

    this.orderItemService.getOrderItemsPage(this.orderId, filter).subscribe({
      next:(value:DataPage<any>) => {
        this.totalRecords = value.totalRecords;
        this.orderItems = value.data;
        this.loadingOrderItems = false;
      },
      error:(err:any) => {
        this.loadingOrderItems = false;
        this.orderItems = [];
        this.first = 0;
      }
    });
  }

  refreshOrderItems(action:CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.loadItems({ ...this.lastLoadEvent, first:this.first, rows:this.rows});
        break;
      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.rows);

        if((pageNumber - 1) * this.rows == this.totalRecords - 1 && pageNumber > 1) {
          this.loadItems({ ...this.lastLoadEvent, first: first-this.rows, rows: this.rows });
        } else {
          this.loadItems({ ...this.lastLoadEvent, first: this.first, rows: this.rows });
        }

        break;
    }
  }

  clearFilters(table:Table): void {
    table.clear();
  }

  setCategories(): void {
    this.orderItemService.getCategories().subscribe((result:any) => {
      if(result.actionSuccess) {
        this.categories = result.actionData;
      }
    })
  }

  setItems(category?:any): void {
    this.orderItemService.getItems(category).subscribe((result:any) => {
      if(result.actionSuccess) {
        this.items = result.actionData;
      }
    });
  }

  showAddOrderItem(): void {
    if(this.disableInput)
      return;

    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.ref=this.dialogService.open(CreateOrderItemComponent,{
        header:translations['ProductAdministration']['AddItemLabel'],
        maximizable: true,
        data:{
          orderId: this.orderId,
          formSubmitted:this.onDialogClose,
          orderItem: null
        }
      });
    });
  }

  showChangeAmount(orderItem: any):void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.ref=this.dialogService.open(CreateOrderItemComponent,{
        header: translations['UserAdministration']['UpdateUserLabel'],
        maximizable: true,
        data:{
          orderId: this.orderId,
          orderItem,
          formSubmitted:this.onDialogClose
        }
      });
    });
  }

  showDeleteConfirm(id:number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.confirmationService.confirm({
        message: translations['Equipment']['DeleteEquipmentConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.orderItemService.deleteOrderItem(id).subscribe({
            next:(msg) => {
              this.refreshOrderItems(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['Equipment'][msg.actionData], key: this.toastKey });
            }
          })
        },
        reject: (type:any) => {
            this.confirmationService.close();
        }
      });
    });
  }

  showDeleteSelectedConfirm(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe((translations: any) =>{
      this.confirmationService.confirm({
        message: translations['Equipment']['DeleteEquipmentSelectedConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.orderItemService.deleteSelectedOrderItems(this.selectedEquipments).subscribe({
            next:(msg:any) => {
              this.selectedEquipments = [];
              this.refreshOrderItems(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['Equipment'][msg.actionData], key: this.toastKey });
            }
          })
        },
        reject: (type:any) => {
            this.confirmationService.close();
        }
      });
    });
  }

  editCompleted(event: any){
    this.orderItemService.changeAmount(this.orderId, this.orderItems[event.index].id, this.orderItems[event.index].amount).subscribe({
      next: (a:any) => {
        this.order.dateModified = new Date(Date.now()).toJSON();
      },
      error: (err: any) => {
        this.orderItems[event.index].amount = event.data;
      }
    });
  }
}
