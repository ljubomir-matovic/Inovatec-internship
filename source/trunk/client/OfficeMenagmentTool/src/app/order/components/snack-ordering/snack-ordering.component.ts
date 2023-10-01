import { ChangeDetectorRef, Component } from '@angular/core';
import { ConfirmationService, LazyLoadEvent, MessageService } from 'primeng/api';
import { CategoriesService } from 'src/app/products/services/categories.service';
import { ItemService } from 'src/app/products/services/item.service';
import { CategoryTypes } from 'src/app/shared/constants/category-types';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Category } from 'src/app/shared/models/category';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Item } from 'src/app/shared/models/item';
import { ItemFilter } from 'src/app/shared/models/item-filter.model';
import { Order } from 'src/app/shared/models/order';
import { OrderFilter } from 'src/app/shared/models/order-filter';
import { OrderRequestService } from '../../services/order-request.service';
import { TranslateLoader, TranslateService } from '@ngx-translate/core';
import { SnackOrder } from 'src/app/shared/models/snack-order';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { Router } from '@angular/router';
import { ROLES, Role } from 'src/app/shared/constants/role';
import { InputNumber } from 'primeng/inputnumber';
import { SignalRService } from 'src/app/shared/services/signalR.service';
import { SignalREvents } from 'src/app/shared/constants/signalREvents';

@Component({
  selector: 'app-snack-ordering',
  templateUrl: './snack-ordering.component.html',
  styleUrls: ['./snack-ordering.component.scss']
})
export class SnackOrderingComponent {
  snacks!: Item[];
  totalSnacks: number = 0;
  pageSize: number = 7;
  loadingAllSnacks!: boolean;
  lazyLoadEvent!: LazyLoadEvent;
  categories!: Category[];
  selectedCategories!: number[];

  mySnackOrders: Order[] = [];
  loadingMySnackOrders!: boolean;
  totalMySnacks: number = 0;
  mySnacksPageSize: number = 8;
  mySnackOrderslazyLoadEvent!: LazyLoadEvent

  officeOrders!: Order[];
  loadingOfficeOrders!: boolean;
  officeOrdersLazyLoadEvent!: LazyLoadEvent;
  totalOfficeOrders: number = 0;
  officeOrdersPageSize: number = 10;
  
  role = Role;

  ordering: boolean = false;

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  constructor(
    private itemService: ItemService,
    private categoryService: CategoriesService,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService,
    private orderRequestService: OrderRequestService,
    private messageService: MessageService,
    public storageService: StorageService,
    private router: Router,
    private signalRService: SignalRService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  get toastKey(): string {
    return "toast";
  }

  ngOnInit(): void {
    this.loadCategories();
    this.initializeSignalR();
  }

  private initializeSignalR(): void {
    this.signalRService.reopen()
      .then((connection) => {
        connection.on(SignalREvents.NewSnackOrder, (order: Order, addedAmount: number, userId?: number) => {
          let currentUserId: number | null = this.storageService.getUserData()?.id ?? null;

          let existingMySnackOrder: Order | undefined = this.mySnackOrders.find(o => o.id == order.id);
          let existingOfficeOrder: Order | undefined = this.officeOrders.find(o => o.item!.id == order.item!.id);
          
          if(userId && currentUserId && userId == currentUserId) {
            if(existingMySnackOrder) {
              existingMySnackOrder.amount! = order.amount!;
            }
            else{
              this.mySnackOrders.unshift({ ...order });
            }
          }

          if(existingOfficeOrder) {
            existingOfficeOrder.amount! += addedAmount!;
          }
          else {
            this.officeOrders.unshift({ ...order });
          }

          this.changeDetectorRef.detectChanges();
        });
        
        connection.on(SignalREvents.DeleteSnackOrder, (order: Order, userId?: number) => {
          let currentUserId: number | null = this.storageService.getUserData()?.id ?? null;
          
          let myOrderPosition = this.mySnackOrders.findIndex(o => o.id == order.id);
          let officeOrderPosition = this.officeOrders.findIndex(o => o.item!.id == order.item!.id);
          
          if(userId && currentUserId && userId == currentUserId) {
            if(myOrderPosition >= 0) {
              this.mySnackOrders.splice(myOrderPosition, 1);
            }
          }

          if(officeOrderPosition >= 0) {
            let targetOfficeOrder: Order = this.officeOrders[officeOrderPosition];
            targetOfficeOrder.amount! -= order.amount!;
            if(targetOfficeOrder.amount! <= 0) {
              this.officeOrders.splice(officeOrderPosition, 1);
            }
          }

          this.changeDetectorRef.detectChanges();
        });

        connection.on(SignalREvents.ClearedOrders, () => {
          this.mySnackOrders = [];
          this.officeOrders = [];
          this.messageService.add({ severity: 'info', detail: this.translateService.instant("OrderAdministration.OrdersCleared"), key: this.toastKey, sticky: true });
        });
      }
    );
  }

  private loadCategories(): void {
    this.categoryService.getCategoriesByType(CategoryTypes.Snacks).subscribe({
      next: (response: ActionResultResponse<Category[]>) => {
        if(response.actionSuccess && response.actionData != null) {
          this.categories = response.actionData;
        }
      }
    });
  }

  lazyLoadSnacks(event: LazyLoadEvent): void {
    this.lazyLoadEvent = event;
    this.loadingAllSnacks = true;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let itemFilter: ItemFilter = {
      CategoryType: CategoryTypes.Snacks,
      Categories: this.selectedCategories,
      PageNumber: this.getPageNumber(event.first, this.pageSize),
      PageSize: this.pageSize,
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    this.snacks = [];
    this.itemService.getItemsPage(itemFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Item>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loadingAllSnacks = false;
          this.snacks = [];
        }
        this.totalSnacks = response.actionData.totalRecords;
        this.loadingAllSnacks = false;
        this.snacks = response.actionData.data;
      },
      error: (error: any) => {
        this.loadingAllSnacks = false;
        this.snacks = [];
      }
    });
  }

  filterSnacks(): void {
    this.lazyLoadSnacks(this.lazyLoadEvent);
  }

  lazyLoadMySnackOrders(event: LazyLoadEvent): void {
    this.mySnackOrderslazyLoadEvent = event;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let snackOrderFilter: OrderFilter = {
      Categories: [],
      Users: [this.storageService.getUserData()!.id],
      PageNumber: this.getPageNumber(event.first, this.mySnacksPageSize),
      PageSize: this.mySnacksPageSize,
      SortField: "dateCreated",
      SortOrder: -1
    };

    this.mySnackOrders = [];
    this.loadingMySnackOrders = true;
    this.orderRequestService.getSnackOrders(snackOrderFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Order>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loadingMySnackOrders = false;
          this.totalMySnacks = 0;
        }

        this.loadingMySnackOrders = false;
        this.totalMySnacks = response.actionData.totalRecords;
        this.mySnackOrders = response.actionData.data;
      },
      error: (error: any) => {
        this.loadingMySnackOrders = false;
        this.totalMySnacks = 0;
      }
    });
  }

  addSelectedSnack(newSnack: Item, amount: number,  amountInput?: InputNumber): void {
    let snackOrder: SnackOrder = {
      itemId: newSnack.id,
      amount: amount,
      officeId: this.storageService.getUserData()!.officeId!
    }

    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.ordering = true;
      this.orderRequestService.addSnackOrder(snackOrder).subscribe(
        {
          next: (response: ActionResultResponse<Order>) => {
            if(response.actionSuccess == false || response.actionData == null) {
              this.messageService.add({ severity: 'error', summary: translations['Common']['Error'], detail: translations['OrderAdministration'][0], key: this.toastKey })
            }
            else {
              this.messageService.add({ severity: 'success', summary: translations['Common']['Success'], key: this.toastKey });
              
              if(amountInput) {
                amountInput.writeValue(null);
              }

              // this.lazyLoadMySnackOrders(this.mySnackOrderslazyLoadEvent);
              // this.lazyLoadOfficeSnacks(this.officeOrdersLazyLoadEvent);
            }
            this.ordering = false;
          },
          error: (err: any) => {
            this.ordering = false;
            this.messageService.add({ severity: 'error', summary: translations['Common']['Error'], detail: translations['OrderAdministration'][err.message], key: this.toastKey })
          }
        }
      );
    });
  }

  showDeleteSelectedSnackDialog(id: number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.confirmationService.confirm({
        message: translations['OrderAdministration']['DeleteOrderConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.orderRequestService.deleteSnackOrder(id).subscribe({
            next: (response: ActionResultResponse<string>) => {
              if(response.actionSuccess != true || response.actionData == null) {
                this.messageService.add({ severity: 'error', summary: translations['Common']['Error'], detail: translations['OrderAdministration'][response.errors[0]], key: this.toastKey });
              }
              // this.lazyLoadMySnackOrders(this.mySnackOrderslazyLoadEvent);
              // this.lazyLoadOfficeSnacks(this.officeOrdersLazyLoadEvent);
              this.messageService.add({ severity: 'success', summary: translations['Common']['Success'], key: this.toastKey });
            },
            error: (err: any) => {
              this.messageService.add({ severity: 'error', summary: translations['Common']['Error'], detail: translations['OrderAdministration'][err.message], key: this.toastKey });
            }
          });
        },
        reject: (type:any) => {
          this.confirmationService.close();
        }
      });
    });
  }

  lazyLoadOfficeSnacks(event: LazyLoadEvent): void {
    this.officeOrdersLazyLoadEvent = event;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let snackOrderFilter: OrderFilter = {
      Categories: [],
      PageNumber: this.getPageNumber(event.first, this.officeOrdersPageSize),
      PageSize: this.officeOrdersPageSize,
      SortField: "amount",
      SortOrder: -1,
      Offices: [this.storageService.getUserData()!.officeId]
    };

    this.officeOrders = [];
    this.loadingOfficeOrders = true;
    this.orderRequestService.getSnackOrdersGroupedByItem(snackOrderFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Order>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loadingOfficeOrders = false;
          this.totalOfficeOrders = 0;
        }
        this.loadingOfficeOrders = false;
        this.totalOfficeOrders = response.actionData.totalRecords;
        this.officeOrders = response.actionData.data;
      },
      error: (error: any) => {
        this.loadingOfficeOrders = false;
        this.totalOfficeOrders = 0;
      }
    });
  }

  createOrder(): void {
    if(this.officeOrders.length == 0)
      return;

    this.confirmationService.confirm({
      header: this.translateService.instant("OrderAdministration.CreateOrder"),
      message: this.translateService.instant("OrderAdministration.CreateOrderConfirm"),
      accept: () => {
        this.router.navigateByUrl("/accept-order");
      }
    });
  }
}
