import { ChangeDetectorRef, Component, EventEmitter } from '@angular/core';
import { FilterMatchMode, LazyLoadEvent, SelectItem } from 'primeng/api';
import { Order } from 'src/app/shared/models/order';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { Table } from 'primeng/table';
import { ORDER_STATES, OrderState } from 'src/app/shared/constants/order-states';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { ROLES, Role } from 'src/app/shared/constants/role';
import { OpenReportComponent } from '../open-report/open-report.component';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { ActivatedRoute } from '@angular/router';
import { ReportService } from '../../services/report.service';
import { ReportFilter } from 'src/app/shared/models/report-filter';
import { Report } from 'src/app/shared/models/report';
import { REPORT_CATEGORIES } from 'src/app/shared/constants/report-categories';
import { Office } from 'src/app/shared/models/office';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { OfficeService } from 'src/app/office/services/office.service';

@Component({
  selector: 'app-reports-administration',
  templateUrl: './reports-administration.component.html',
  styleUrls: ['./reports-administration.component.scss']
})
export class ReportsAdministrationComponent {
  reports: Report[] = [];
  lastLoadEvent!: LazyLoadEvent;
  loading: boolean = true;

  forCurrentUserOnly: boolean = false;

  UserRole = Role;
  userRoles = ROLES
  orderState = OrderState;
  orderStates: any = ORDER_STATES;
  reportCategories = REPORT_CATEGORIES;

  offices!: Office[];

  selectedStates: number[] = [this.orderStates[OrderState.Pending], this.orderStates[OrderState.InProgress]];

  pageSize: number = 10;
  totalRecords: number = 1;
  first: number = 0;

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
    private reportsService: ReportService,
    private dialogService: DialogService,
    private officeService: OfficeService,
    private changeDetectorRef: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.dialogRef = new DynamicDialogRef();
    this.dialogStatus = new EventEmitter<any>();

    this.getOffices();

    this.initSubscriptions();
    this.forCurrentUserOnly = this.route.snapshot.data["forCurrentUserOnly"] ?? false;
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

  loadReports(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || this.pageSize === undefined)
      return;

    let reportFilter: ReportFilter = {
      Name: '',
      Description: '',
      PageNumber: this.getPageNumber(this.first, this.pageSize),
      PageSize: this.pageSize,
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      reportFilter.Name = event.filters["name"]?.value ?? null;
      reportFilter.Description = event.filters["description"]?.value ?? null;
      reportFilter.Categories = event.filters["categories"]?.value?.map((x: any) => x.id) ?? null;
      reportFilter.Offices = event.filters["offices"]?.value?.map((x: any) => x.id) ?? null;
    }

    reportFilter.States = this.selectedStates.map((x: any) => x.id) ?? null;

    if(this.forCurrentUserOnly) {
      reportFilter.Users = [this.storageService.getUserData()!.id];
    }

    this.reports = [];
    this.reportsService.getReports(reportFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Report>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loading = false;
          this.reports = [];
          this.first = 0;
          return;
        }

        this.totalRecords = response.actionData.totalRecords;
        this.reports = response.actionData.data;
        this.loading = false;
      },
      error: (error: any) => {
        this.loading = false;
        this.reports = [];
        this.first = 0;
      }
    });
  }

  refreshReports(action: CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.loadReports({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        break;

      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.pageSize);

        if((pageNumber - 1) * this.pageSize == this.totalRecords - 1 && pageNumber > 1){
          this.loadReports({ ...this.lastLoadEvent, first: first-this.pageSize, rows: this.pageSize });
        }
        else {
          this.loadReports({ ...this.lastLoadEvent, first: this.first, rows: this.pageSize });
        }
        break;
    }
  }

  filterByStates(table: Table): void {
    table.filter(this.selectedStates, "states", this.matchMode);
  }

  filterReports(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  openReport(event: any): void {
    let report: Order = event.data;
    this.dialogRef = this.dialogService.open(
      OpenReportComponent,
      {
        maximizable: false,
        dismissableMask: true,
        data: {
          report,
          formSubmitted: this.onDialogClose
        }
      }
    );
  }

  dialogClosed(status: any): void {
    switch (status.data.action) {
      case CRUDActions.Update:
        let receivedReport: Report = status.data.report;
        let reportIndex: number = this.reports.findIndex(report => report.id == receivedReport.id);
        if(reportIndex != -1) {
          this.reports[reportIndex] = receivedReport;
          this.changeDetectorRef.detectChanges();
        }
        break;

      case CRUDActions.Delete:
        this.refreshReports(CRUDActions.Delete);
        this.dialogRef.close();
        break;

      default:
        break;
    }
  }
}
