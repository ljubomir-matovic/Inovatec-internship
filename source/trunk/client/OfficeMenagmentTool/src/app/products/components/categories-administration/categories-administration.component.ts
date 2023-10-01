import { Component, EventEmitter } from '@angular/core';
import { ConfirmationService, FilterMatchMode, LazyLoadEvent, MessageService, SelectItem } from 'primeng/api';
import { CategoriesService } from '../../services/categories.service';
import { Category } from 'src/app/shared/models/category';
import { CATEGORY_TYPES } from 'src/app/shared/constants/category-types';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CreateCategoryComponent } from '../create-category/create-category.component';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { Table } from 'primeng/table';
import { CategoryFilter } from 'src/app/shared/models/category-filter.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-categories-administration',
  templateUrl: './categories-administration.component.html',
  styleUrls: ['./categories-administration.component.scss']
})
export class CategoriesAdministrationComponent {
  categories: Category[] = [];
  lastLoadEvent!: LazyLoadEvent;
  loading: boolean = true;
  dynamicDialogRef!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;

  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();

  categoryTypes: any[] = CATEGORY_TYPES;
  categoryType: number = 0;

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

  get toastKey(): string {
    return "toast";
  }

  constructor(
    private categoryService: CategoriesService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
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

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  loadCategories(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first == undefined || this.pageSize == undefined)
      return;

    let categoryFilter: CategoryFilter = {
      Name: '',
      Types: [],
      PageNumber: this.getPageNumber(event.first, this.pageSize),
      PageSize: this.pageSize,
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      categoryFilter.Name = event.filters["name"]?.value ?? null;
      categoryFilter.Types = event.filters["types"]?.value?.map((x: any) => x.id) ?? null;
    }

    this.categories = [];
    this.categoryService.getFilteredCategories(categoryFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Category>>) => {
        if(response.actionSuccess != true || response.actionData == null)
          return;

        this.totalRecords = response.actionData.totalRecords;
        this.loading = false;
        this.categories = response.actionData.data;
      },
      error: (error: any) => {
        this.loading = false;
        this.categories = [];
        this.first = 0;
      }
    });
  }

  refreshCategories(action: CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.loadCategories({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        break;

      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.pageSize);

        if((pageNumber - 1) * this.pageSize == this.totalRecords - 1 && pageNumber > 1) {
          this.loadCategories({ ...this.lastLoadEvent, first: first-this.pageSize, rows: this.pageSize });
        }
        else {
          this.loadCategories({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        }
        break;
    }
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  filterCategories(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  dialogClosed(status: ActionResult<any>): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      if(status.success) {
        this.refreshCategories(status.data.actions);
        this.dynamicDialogRef.close();
        this.messageService.add({ severity: "success", summary: translations['Common']["Success"], detail: translations['CategoryAdministration'][status.data.message], key: this.toastKey });
      }
      else {
        this.messageService.add({ severity: "error", summary: translations['Common']["Error"], detail: translations['CategoryAdministration'][status.data.message], key: this.toastKey });
      }
    });
  }

  showAddCategory(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.dynamicDialogRef = this.dialogService.open(CreateCategoryComponent, {
        header: translations['CategoryAdministration']['AddCategoryLabel'],
        maximizable: false,
        data: { categorySubmitted: this.onDialogClose }
      });
    });
  }

  showUpdateCategory(category: Category): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.dynamicDialogRef = this.dialogService.open(CreateCategoryComponent, {
        header: translations['CategoryAdministration']['UpdateCategoryLabel'],
        maximizable: false,
        data: { category: category, categorySubmitted: this.onDialogClose }
      });
    });
  }

  showDeleteConfirm(id: number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.confirmationService.confirm({
        message: translations['CategoryAdministration']['DeleteCategoryConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.categoryService.deleteCategory(id).subscribe({
            next:(response: any) => {
              this.refreshCategories(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['CategoryAdministration'][response.actionData], key: this.toastKey });
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
