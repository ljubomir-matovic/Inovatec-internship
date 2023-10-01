import { ChangeDetectorRef, Component, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, FilterMatchMode, LazyLoadEvent, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Office } from 'src/app/shared/models/office';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { OfficeService } from '../../services/office.service';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { CreateOfficeComponent } from '../create-office/create-office.component';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-office-administration',
  templateUrl: './office-administration.component.html',
  styleUrls: ['./office-administration.component.scss']
})
export class OfficeAdministrationComponent {
  lazyLoadEvent!: LazyLoadEvent;
  offices: Office[] = [];
  totalRecords: number = 1;
  first: number = 0;
  rows: number = 10;
  position: string = "";
  loading: boolean=true;

  dynamicDialogRef!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  subscriptions: EventEmitter<any>[] = [];
  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();

  get toastKey():string {
    return "toast";
  }

  constructor(
    private officeService: OfficeService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private translateService: TranslateService,
    private changeDetectorRef: ChangeDetectorRef
    ) {}

  ngOnInit(): void {
    this.initSubscriptions();
    this.dynamicDialogRef = new DynamicDialogRef();
    this.dialogStatus=new EventEmitter<any>();
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

  getPageNumber(first: number, rows: number): number {
    return ( first / rows ) + 1;
  }

  lazyLoadOffices(event: LazyLoadEvent): void {
    this.lazyLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || event.rows === undefined)
      return;

    let officeFilter: OfficeFilter = {
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1,
    };

    if(event.filters !== undefined) {
      officeFilter.Name = event.filters["name"]?.value ?? null;
    }

    this.offices = [];
    this.officeService.getOffices(officeFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Office>>) => {
        this.totalRecords = response.actionData.totalRecords;
        this.offices = response.actionData.data;
        this.loading = false;
      },
      error: (err: any) => {
        this.loading = false;
        this.offices = [];
        this.first = 0;
      }
    });
  }

  refreshOffices(action: CRUDActions, updatedOffice?: Office): void {
    switch(action) {
      case CRUDActions.Create:
        this.lazyLoadOffices({ ...this.lazyLoadEvent, first: this.first, rows: this.rows});
        break;
      case CRUDActions.Update:
        let index = this.offices.findIndex(office => office.id === updatedOffice!.id);
        this.offices[index] = updatedOffice!;
        this.changeDetectorRef.detectChanges();
        break;
      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.rows);

        if((pageNumber - 1) * this.rows == this.totalRecords - 1 && pageNumber > 1){
          this.lazyLoadOffices({ ...this.lazyLoadEvent, first: first-this.rows, rows: this.rows });
        } else {
          this.lazyLoadOffices({ ...this.lazyLoadEvent, first: this.first, rows: this.rows });
        }

        break;
    }
  }

  showAddOffice(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.dynamicDialogRef = this.dialogService.open(CreateOfficeComponent, {
        header: this.translateService.instant('OfficeAdministration.AddOfficeLabel'),
        data: {
          officeSubmitted: this.onDialogClose
        }
      });
    });
  }

  showUpdateOffice(office: Office): void {
    console.log("TEST")
    this.dynamicDialogRef = this.dialogService.open(CreateOfficeComponent, {
      header: this.translateService.instant('OfficeAdministration.UpdateOfficeLabel'),
      data: {
        office,
        officeSubmitted: this.onDialogClose
      }
    });
  }

  showDeleteConfirm(id: number): void {
    this.confirmationService.confirm({
      message: this.translateService.instant('OfficeAdministration.DeleteOfficeConfirmationDescription'),
      header: this.translateService.instant('Common.DeleteConfirmationTitle'),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.officeService.deleteOffice(id).subscribe({
          next: (message) => {
            this.refreshOffices(CRUDActions.Delete);
            this.confirmationService.close();
            this.messageService.add({ severity: 'success', summary: this.translateService.instant('Common.Success'), detail: this.translateService.instant(`OfficeAdministration.${message.actionData}`), key: this.toastKey });
          }
        })
      },
      reject: (type: any) => {
          this.confirmationService.close();
      }
    });
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  applyFilter(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  dialogClosed(status: ActionResult<any>): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      if(status.success) {
        this.refreshOffices(status.data.action, status.data.office);
        this.dynamicDialogRef.close();
        this.messageService.add({ severity: "success", summary: this.translateService.instant('Common.Success'), detail: this.translateService.instant(`OfficeAdministration.${status.data.message}`), key: this.toastKey });
      } else {
        this.messageService.add({ severity: "error", summary: this.translateService.instant('Common.Error'), detail: this.translateService.instant(`OfficeAdministration.${status.data.message}`), key: this.toastKey });
      }
    });
  }
}
