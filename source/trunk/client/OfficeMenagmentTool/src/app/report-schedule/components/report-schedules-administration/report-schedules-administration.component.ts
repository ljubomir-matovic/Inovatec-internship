import { ChangeDetectorRef, Component, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ConfirmationService, FilterMatchMode, LazyLoadEvent, MessageService, SelectItem } from "primeng/api";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { Table } from "primeng/table";
import { ActionResult } from "src/app/shared/models/action-result.model";
import { Office } from "src/app/shared/models/office";
import { ReportSchedule } from "src/app/shared/models/report-schedule";
import { ReportScheduleService } from "../../services/report-schedule.service";
import { ReportScheduleFilter } from "src/app/shared/models/report-schedule-filter";
import { DataPage } from "src/app/shared/models/data-page.model";
import { TranslateService } from "@ngx-translate/core";
import { CRUDActions } from "src/app/shared/models/crud-actions.model";
import { CreateReportScheduleComponent } from "../create-report-schedule/create-report-schedule.component";
import { OfficeFilter } from "src/app/shared/models/office-filter.model";
import { ActionResultResponse } from "src/app/shared/models/action-result-response.model";
import { OfficeService } from "src/app/office/services/office.service";
import { StorageService } from "src/app/shared/helpers/storage.service";
import { TRUE_FALSE_ANY_FILTER_OPTIONS, TrueFalseAnyFilter } from "src/app/shared/constants/true-false-any.filter";

@Component({
  selector: 'app-report-schedules-administration',
  templateUrl: './report-schedules-administration.component.html',
  styleUrls: ['./report-schedules-administration.component.scss']
})
export class ReportSchedulesAdministrationComponent {
  reportSchedules: ReportSchedule[] = [];
  lazyLoadEvent!: LazyLoadEvent;
  loading!: boolean;

  forCurrentUserOnly: boolean = false;

  offices!: Office[];

  pageSize: number = 10;
  totalRecords: number = 1;
  first: number = 0;

  selectedOffices!: Office[];

  selectedStates!: number[];
  trueFalseOptions = TRUE_FALSE_ANY_FILTER_OPTIONS;

  subscriptions: EventEmitter<any>[] = [];
  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();
  dynamicDialogRef!: DynamicDialogRef;
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
    private reportScheduleService: ReportScheduleService,
    private storageService: StorageService,
    private translateService: TranslateService,
    private officeService: OfficeService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private dialogService: DialogService,
    private changeDetectorRef: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.dynamicDialogRef = new DynamicDialogRef();
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

          this.setUserOfficeAsSelected();
        },
        error: (error: any) => {
          this.offices = [];
          this.messageService.add({ severity: 'error', summary: this.translateService.instant('Common', 'Error'), key: this.toastKey })
        }
      }
    );
  }

  setUserOfficeAsSelected(): void {
    let officeId: number = this.storageService.getUserData()!.officeId!;
    this.selectedOffices = this.offices.filter(office => office.id == officeId);

    this.manualyLoadReportSchedules();
  }

  manualyLoadReportSchedules(): void {
    this.first = 0;
    this.loading = true;

    let filter: ReportScheduleFilter = {
      PageSize: this.pageSize,
      PageNumber: 1,
      SortField: null,
      SortOrder: 1,
      State: TrueFalseAnyFilter.Any
    };

    if(this.selectedOffices) {
      filter.Offices = this.selectedOffices.map((x: any) => x.id) ?? null;
    }

    this.reportSchedules = [];
    this.reportScheduleService.getReportSchedules(filter).subscribe({
      next: (dataPage: DataPage<ReportSchedule>) => {
        this.reportSchedules = dataPage.data;
        this.totalRecords = dataPage.totalRecords;
        this.loading = false;
      },
      error: (error: any) => {
        this.reportSchedules = [];
        this.loading = false;
      }
    });
  }

  lazyLoadReportSchedules(event: LazyLoadEvent): void {
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first == undefined || event.rows == undefined) {
      return;
    }

    let filter: ReportScheduleFilter = {
      PageSize: this.pageSize,
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder && event.sortOrder < 0) ? -1 : 1,
      State: TrueFalseAnyFilter.Any
    };

    if(event.filters) {
      filter.State = event.filters["state"]?.value ?? TrueFalseAnyFilter.Any;
    }

    if(this.selectedOffices) {
      filter.Offices = this.selectedOffices.map((x: any) => x.id) ?? null;
    }

    this.reportSchedules = [];
    this.reportScheduleService.getReportSchedules(filter).subscribe({
      next: (dataPage: DataPage<ReportSchedule>) => {
        this.reportSchedules = dataPage.data;
        this.totalRecords = dataPage.totalRecords;
        this.loading = false;
      },
      error: (error: any) => {
        this.reportSchedules = [];
        this.loading = false;
      }
    });
  }

  capitalizeFirstLetter(word: string): string {
    return word.charAt(0).toUpperCase() + word.slice(1);
  }

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  refreshReportSchedules(action: CRUDActions, updatedReportSchedule?: ReportSchedule): void {
    switch(action) {
      case CRUDActions.Create:
        this.lazyLoadReportSchedules({ ...this.lazyLoadEvent, first: this.first, rows: this.pageSize});
        break;
      case CRUDActions.Update:
        let index = this.reportSchedules.findIndex(reportSchedule => reportSchedule.id === updatedReportSchedule!.id);
        this.reportSchedules[index] = updatedReportSchedule!;
        this.changeDetectorRef.detectChanges();
        break;
      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.pageSize);

        if((pageNumber - 1) * this.pageSize == this.totalRecords - 1 && pageNumber > 1) {
          this.lazyLoadReportSchedules({ ...this.lazyLoadEvent, first: first-this.pageSize, rows: this.pageSize });
        }
        else {
          this.lazyLoadReportSchedules({ ...this.lazyLoadEvent, first: this.first, rows: this.pageSize });
        }

        break;
    }
  }

  filterByOffices(table: Table): void {
    table.filter(this.selectedOffices, "offices", this.matchMode);
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  dialogClosed(status: any): void {
    if(status.success) {
      this.refreshReportSchedules(status.data.action, status.data.reportSchedule);
      this.dynamicDialogRef.close();
      this.messageService.add({ severity: "success", summary: this.translateService.instant("Common.Success"), detail: this.translateService.instant(`ScheduleAdministration.${status.data.message}`), key: this.toastKey });
    }
    else {
      this.messageService.add({ severity: "error", summary: this.translateService.instant("Common.Error"), detail: this.translateService.instant(`ScheduleAdministration.${status.data.message}`), key: this.toastKey });
    }
  }

  showAddReportSchedule() {
    this.dynamicDialogRef = this.dialogService.open(CreateReportScheduleComponent, {
      header: this.translateService.instant("ScheduleAdministration.AddReportScheduleLabel"),
      data: {
        reportScheduleSubmitted: this.onDialogClose
      }
    });
  }

  showUpdateReportSchedule(reportSchedule: ReportSchedule) {
    this.dynamicDialogRef = this.dialogService.open(CreateReportScheduleComponent, {
      header: this.translateService.instant("ScheduleAdministration.EditReportScheduleLabel"),
      data: {
        reportSchedule: reportSchedule,
        reportScheduleSubmitted: this.onDialogClose
      }
    });
  }

  filterReportSchedulesByState(event: any, table: Table, field: string) {
    table.filter(event.itemValue.value, field, this.matchMode);
  }

  filterReportSchedules(event: any, table: Table, field: string) {
    table.filter(event.target.value, field, this.matchMode);
  }

  showDeleteConfirm(id: number) {
    this.confirmationService.confirm({
      message: this.translateService.instant("ScheduleAdministration.DeleteReportScheduleConfirmationDescription"),
      header: this.translateService.instant("Common.DeleteConfirmationTitle"),
      icon: 'pi pi-info-circle',
      accept: () => {
        this.reportScheduleService.deleteReportSchedule(id).subscribe({
          next:(response: any) => {
            this.refreshReportSchedules(CRUDActions.Delete);
            this.confirmationService.close();
            this.messageService.add({ severity: "success", summary: this.translateService.instant("Common.Success"), detail: this.translateService.instant(`ScheduleAdministration.${response.actionData}`), key: this.toastKey });
          }
        });
      },
      reject: () => {
        this.confirmationService.close();
      }
    });
  }
}
