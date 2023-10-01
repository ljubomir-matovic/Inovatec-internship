import { Component, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Category } from 'src/app/shared/models/category';
import { CategoriesService } from '../../services/categories.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Item } from 'src/app/shared/models/item';
import { ConfirmationService, FilterMatchMode, LazyLoadEvent, MessageService, SelectItem } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { ItemFilter } from 'src/app/shared/models/item-filter.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { ItemService } from '../../services/item.service';
import { Table } from 'primeng/table';
import { CreateItemComponent } from '../create-item/create-item.component';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-products-administration',
  templateUrl: './products-administration.component.html',
  styleUrls: ['./products-administration.component.scss']
})
export class ProductsAdministrationComponent {
  categories!: Category[];
  selectedCategories!: number[];

  categoryType!: number;

  items: Item[] = [];
  lastLoadEvent!: LazyLoadEvent;
  loading: boolean = true;
  dynamicDialogRef!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;

  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();

  pageSize: number = 10;
  totalRecords: number = 1;
  first: number = 0;

  get matchModelOptions(): SelectItem[] {
    return [{ label: "Contains", value: FilterMatchMode.CONTAINS }];
  }

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  subscriptions: EventEmitter<any>[] = [];

  get toastKey():string {
    return "toast";
  }

  constructor(
    private route: ActivatedRoute,
    private categoryService: CategoriesService,
    private itemService: ItemService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.categoryType = this.route.snapshot.data["categoryType"];
    this.loadCategories();

    this.initSubscriptions();
    this.dynamicDialogRef = new DynamicDialogRef();
    this.dialogStatus = new EventEmitter<any>();
  }

  initSubscriptions(): void {
    this.subscriptions.push(this.onDialogClose);
    this.onDialogClose.subscribe({
      next: (status: ActionResult<any>) => {
        this.dialogClosed(status);
      }
    });
  }

  private loadCategories(): void {
    this.categoryService.getCategoriesByType(this.categoryType).subscribe({
      next: (response: ActionResultResponse<Category[]>) => {
        if(!response.actionSuccess || response.actionData == null)
          return;
        this.categories = [];
        response.actionData.forEach(category => {
          this.categories.push(category);
        });
      }
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  loadItems(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let itemFilter: ItemFilter = {
      Name: '',
      CategoryType: this.categoryType,
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
        if(response.actionSuccess != true || response.actionData == null)
          return;

          this.totalRecords = response.actionData.totalRecords;
          this.loading = false;
          this.items = response.actionData.data;
      },
      error: (error: any) => {
        this.loading = false;
        this.items = [];
        this.first = 0;
      }
    });
  }

  refreshItems(action: CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.loadItems({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        break;

      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.pageSize);

        if((pageNumber - 1) * this.pageSize == this.totalRecords - 1 && pageNumber > 1){
          this.loadItems({ ...this.lastLoadEvent, first: first-this.pageSize, rows: this.pageSize });
        }
        else {
          this.loadItems({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        }
        break;
    }
  }

  filterItems(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  dialogClosed(status: ActionResult<any>): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      if(status.success) {
        this.refreshItems(status.data.actions);
        this.dynamicDialogRef.close();
        this.messageService.add({ severity: "success", summary: translations['Common']["Success"], detail: translations['ProductAdministration'][status.data.message], key: this.toastKey });
      }
      else {
        this.messageService.add({ severity: "error", summary: translations['Common']["Error"], detail: translations['ProductAdministration'][status.data.message], key: this.toastKey });
      }
    });
  }

  showAddItem(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.dynamicDialogRef = this.dialogService.open(CreateItemComponent, {
        header: translations['ProductAdministration']['AddItemLabel'],
        maximizable: false,
        data: { itemSubmitted: this.onDialogClose, categories: this.categories }
      });
    });
  }

  showUpdateItem(item: Item): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.dynamicDialogRef = this.dialogService.open(CreateItemComponent, {
        header: translations['ProductAdministration']['UpdateItemLabel'],
        maximizable: false,
        data: { item: item, itemSubmitted: this.onDialogClose, categories: this.categories }
      });
    });
  }

  showDeleteConfirm(id: number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.confirmationService.confirm({
        message: translations['ProductAdministration']['DeleteItemConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.itemService.deleteItem(id).subscribe({
            next:(response: any) => {
              this.refreshItems(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['ProductAdministration'][response.actionData], key: this.toastKey });
            }
          });
        },
        reject: () => {
          this.confirmationService.close();
        }
      });
    });
  }
}
