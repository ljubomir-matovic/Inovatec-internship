import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Item } from 'src/app/shared/models/item';
import { FilterMatchMode, LazyLoadEvent, MessageService } from 'primeng/api';
import { CategoryTypes } from 'src/app/shared/constants/category-types';
import { CategoriesService } from 'src/app/products/services/categories.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Category } from 'src/app/shared/models/category';
import { ItemFilter } from 'src/app/shared/models/item-filter.model';
import { ItemService } from 'src/app/products/services/item.service';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Table } from 'primeng/table';
import { EquipmentOrder } from 'src/app/shared/models/equipment-order';
import { AppRoutes } from 'src/app/shared/constants/routes';
import { Router } from '@angular/router';
import { OrderRequestService } from '../../services/order-request.service';

@Component({
  selector: 'app-order-equipment',
  templateUrl: './order-equipment.component.html',
  styleUrls: ['./order-equipment.component.scss']
})
export class OrderEquipmentComponent {
  submittingOrder: boolean = false;

  categories!: Category[];
  selectedCategories!: number[];

  items!: Item[];
  lazyLoadEvent!: LazyLoadEvent;
  totalRecords!: number;
  pageSize: number = 5;
  first: number = 0;
  loadingItems!: boolean;
  selectedItem: Item | null = null;

  orderDescription: string = '';

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  constructor(
    private translateService: TranslateService,
    private itemService: ItemService,
    private categoryService: CategoriesService,
    private orderRequestService: OrderRequestService,
    private messageService: MessageService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  get toastKey(): string {
    return "toast";
  }

  loadCategories(): void {
    this.categoryService.getCategoriesByType(CategoryTypes.Equipment).subscribe({
      next: (response: ActionResultResponse<Category[]>) => {
        if(!response.actionSuccess || response.actionData == null) {
          this.categories = [];
          return;
        }

        this.categories = response.actionData;
      },
      error: (error: any) => {
        this.categories = [];
      }
    });
  }

  lazyLoadItems(event: LazyLoadEvent): void {
    this.lazyLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loadingItems = true;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let itemFilter: ItemFilter = {
      Name: '',
      CategoryType: CategoryTypes.Equipment,
      Categories: this.selectedCategories,
      PageNumber: this.getPageNumber(event.first, this.pageSize),
      PageSize: this.pageSize,
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      itemFilter.Name = event.filters["name"]?.value ?? null;
      itemFilter.Categories = event.filters["categories"]?.value?.map((x: any) => x.id) ?? null;
    }

    this.items = [];
    this.itemService.getItemsPage(itemFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Item>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loadingItems = false;
          this.items = [];
          this.first = 0;
          return;
        }

        this.totalRecords = response.actionData.totalRecords;
        this.loadingItems = false;
        this.items = response.actionData.data;
      },
      error: (error: any) => {
        this.loadingItems = false;
        this.items = [];
        this.first = 0;
      }
    });
  }

  filterItems(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  submitOrder(): void {
    if(this.selectedItem == null || this.orderDescription.trim() == '' || this.submittingOrder == true) {
      return;
    }

    let orderData: EquipmentOrder = {
      itemId: this.selectedItem.id,
      description: this.orderDescription
    }

    this.submittingOrder = true;
    this.orderRequestService.orderEquipment(orderData).subscribe({
      next: (actionResultResponse: ActionResultResponse<string>) => {
        if(actionResultResponse == null || !actionResultResponse.actionSuccess) {
          this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
            this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['OrdersAdministration'][actionResultResponse.errors[0]], key: this.toastKey });
          });
        }
        this.submittingOrder = false;

        this.router.navigate([AppRoutes.shared.myEquipmentOrders]);
      },
      error: (error: any) => {
        this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
          this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['Common'][error.message], key: this.toastKey });
        });
      }
    });
  }
}
