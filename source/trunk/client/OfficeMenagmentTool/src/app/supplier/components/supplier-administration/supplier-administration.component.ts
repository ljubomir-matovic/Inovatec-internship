import { ChangeDetectorRef, Component, EventEmitter } from '@angular/core';
import { ConfirmationService, FilterMatchMode, LazyLoadEvent, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { Supplier } from 'src/app/shared/models/supplier';
import { SupplierService } from '../../services/supplier.service';
import { TranslateService } from '@ngx-translate/core';
import { SupplierFilter } from 'src/app/shared/models/supplier-filter';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { CreateSupplierComponent } from '../create-supplier/create-supplier.component';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-supplier-administration',
  templateUrl: './supplier-administration.component.html',
  styleUrls: ['./supplier-administration.component.scss']
})
export class SupplierAdministrationComponent {
  lastLoadEvent!: LazyLoadEvent;
  suppliers: Supplier[] = [];
  totalRecords: number = 1;
  loading: boolean=true;
  ref!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;
  first: number = 0;
  rows: number = 10;
  position: string = "";

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  subscriptions: EventEmitter<any>[] = [];
  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();

  get toastKey():string {
    return "toast";
  }

  constructor(
    private supplierService: SupplierService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private translateService: TranslateService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initSubscriptions();
    this.ref = new DynamicDialogRef();
    this.dialogStatus = new EventEmitter<any>();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  getPageNumber(first: number, rows: number):number {
    return ( first / rows ) + 1;
  }

  initSubscriptions(): void {
    this.subscriptions.push(this.onDialogClose);

    this.onDialogClose.subscribe({
      next: (status: ActionResult<any>) => {
        this.dialogClosed(status);
      }
    });
  }

  dialogClosed(status: ActionResult<any>): void {
    if(status.success) {
      if(status.data.supplier) {
        this.refreshSuppliers(status.data.action, status.data.supplier);
      }
      else {
        this.refreshSuppliers(status.data.action);
      }
      this.ref.close();
      this.messageService.add({ severity: "success", summary: this.translateService.instant('Common.Success'), detail: this.translateService.instant(`SupplierAdministration.${status.data.message}`), key: this.toastKey });
    } 
    else {
      this.messageService.add({ severity: "error", summary: this.translateService.instant('Common.Error'), detail: this.translateService.instant(`SupplierAdministration.${status.data.message}`), key: this.toastKey });
    }
  }

  loadSuppliers(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || event.rows === undefined)
      return;

    let filter: SupplierFilter = {
      PageSize: this.rows,
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      filter.PhoneNumber = event.filters["phoneNumber"]?.value ?? null;
      filter.Country = event.filters["country"]?.value ?? null;
      filter.City = event.filters["city"]?.value ?? null;
      filter.Address = event.filters["address"]?.value ?? null;
    }
    this.suppliers = [];

    this.supplierService.getSuppliers(filter).subscribe({
      next:(value: DataPage<Supplier>) => {
        this.totalRecords = value.totalRecords;
        this.suppliers = value.data;
        this.loading = false;
      },
      error:(err:any) => {
        this.loading = false;
        this.suppliers = [];
        this.first = 0;
      }
    });
  }

  refreshSuppliers(action: CRUDActions, updatedSupplier?: Supplier): void {
    switch(action){
      case CRUDActions.Create:
        this.loadSuppliers({ ...this.lastLoadEvent, first: this.first, rows: this.rows});
        break;
      case CRUDActions.Update:
        let index = this.suppliers.findIndex(supplier => supplier.id === updatedSupplier!.id);
        this.suppliers[index] = updatedSupplier!;
        this.changeDetectorRef.detectChanges();
        break;
      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.rows);

        if((pageNumber - 1) * this.rows == this.totalRecords - 1 && pageNumber > 1){
          this.loadSuppliers({ ...this.lastLoadEvent, first: first-this.rows, rows: this.rows });
        } 
        else {
          this.loadSuppliers({ ...this.lastLoadEvent, first: this.first, rows: this.rows });
        }

        break;
    }
  }

  showAddSupplier(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.ref = this.dialogService.open(CreateSupplierComponent, {
        header: translations['SupplierAdministration']['AddSupplierLabel'],
        maximizable: false,
        style: { 'width': '100%', 'max-width': '30em'},
        data: {
          formSubmitted: this.onDialogClose
        }
      });
    });
  }

  showUpdateSupplier(supplier: Supplier): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.ref = this.dialogService.open(CreateSupplierComponent,{
        header: translations['SupplierAdministration']['UpdateSupplierLabel'],
        maximizable: false,
        style: { 'width': '100%', 'max-width': '30em'},
        data: {
          supplier: supplier,
          formSubmitted: this.onDialogClose
        }
      });
    });
  }

  showDeleteConfirm(id: number): void {
    this.confirmationService.confirm({
      message: this.translateService.instant('SupplierAdministration.DeleteSupplierConfirmationDescription'),
      header: this.translateService.instant('Common.DeleteConfirmationTitle'),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.supplierService.deleteSupplier(id).subscribe({
          next: (message) => {
            this.refreshSuppliers(CRUDActions.Delete);
            this.confirmationService.close();
            this.messageService.add({ severity: 'success', summary: this.translateService.instant('Common.Success'), detail: this.translateService.instant(`SupplierAdministration.${message.actionData}`), key: this.toastKey });
          }
        })
      },
      reject: (type: any) => {
        this.confirmationService.close();
      }
    });
  }

  clearFilters(table: Table):void {
    table.clear();
  }

  applyFilter(event:any, table:Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }
}