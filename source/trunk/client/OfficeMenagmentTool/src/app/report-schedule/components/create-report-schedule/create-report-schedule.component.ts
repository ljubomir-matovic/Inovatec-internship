import { Component, EventEmitter, ViewChild } from '@angular/core';
import { OfficeService } from 'src/app/office/services/office.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Office } from 'src/app/shared/models/office';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { ReportScheduleService } from '../../services/report-schedule.service';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { HttpErrorResponse } from '@angular/common/http';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { UpdateReportSchedule } from 'src/app/shared/models/update-report-schedule-model';
import { ReportSchedule } from 'src/app/shared/models/report-schedule';
import { Calendar } from 'primeng/calendar';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-create-report-schedule',
  templateUrl: './create-report-schedule.component.html',
  styleUrls: ['./create-report-schedule.component.scss']
})
export class CreateReportScheduleComponent {
  @ViewChild("calendar", { static: true }) calendar!: Calendar;

  reportSchedule: ReportSchedule | null = null;

  offices!: Office[];

  reportScheduleForm!: FormGroup;
  loading: boolean = false;
  reportScheduleSubmitted!: EventEmitter<ActionResult<any>>;

  minDate!: Date;

  get toastKey():string {
    return "toast";
  }

  get invalidDate(): boolean {
    return this.calendar.minDate > this.reportScheduleForm.controls['scheduleDate'].value && this.reportScheduleForm.controls['isActive'].value == true;
  }

  constructor(
    private officeService: OfficeService,
    private reportScheduleService: ReportScheduleService,
    private storageService: StorageService,
    public dialogConfig: DynamicDialogConfig,
    private messageService: MessageService,
    private translateService: TranslateService,
    private formBuilder: FormBuilder
  ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.reportSchedule) {
      this.reportSchedule = this.dialogConfig.data.reportSchedule;
    }

    this.minDate = new Date(Date.now());
    this.minDate.setTime(this.minDate.getTime() + 1000 * 86400);
    
    this.loadOffices();
    
    this.reportScheduleForm = this.formBuilder.group({
      officeId: [this.reportSchedule?.office.id, Validators.required],
      scheduleDate: [this.minDate, Validators.required],
      isActive: [this.reportSchedule?.isActive]
    });

    if(this.dialogConfig.data?.reportSchedule) {
      this.reportScheduleForm.controls['scheduleDate'].setValue( new Date(this.reportSchedule!.scheduleDate) );
    }
    
    this.reportScheduleSubmitted = this.dialogConfig.data.reportScheduleSubmitted;
  }

  loadOffices(): void {
    let officeFilter: OfficeFilter = {
      PageNumber: 0,
      PageSize: 0,
      SortField: 'name',
      SortOrder: 1
    }
    
    this.offices = [];
    this.officeService.getOffices(officeFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Office>>) => {
        if(response.actionSuccess) {
          this.offices = response.actionData.data;
          if(this.reportSchedule?.office.id == undefined) {
            this.reportScheduleForm.controls['officeId'].setValue(this.storageService.getUserData()!.officeId);
          }
        }
        else{
          this.offices = [];

        }
      },
      error: (_: HttpErrorResponse) => {
        this.offices = [];
        this.messageService.add({ severity: 'error', summary: this.translateService.instant('Common', 'Error'), key: this.toastKey })
      }
    });
  }

  submitForm() {
    let result;
    let action: CRUDActions;
    
    let reportScheduleUpdate: UpdateReportSchedule = {
      scheduleDate: this.reportScheduleForm.value.scheduleDate,
      officeId: this.reportScheduleForm.value.officeId,
      isActive: true
    }

    if(this.reportSchedule == null){
      result = this.reportScheduleService.addReportSchedule(reportScheduleUpdate);
      action = CRUDActions.Create;
    }
    else {
      reportScheduleUpdate.isActive = this.reportScheduleForm.value.isActive;

      result = this.reportScheduleService.updateReportSchedule({ id: this.reportSchedule!.id, ...reportScheduleUpdate});
      action = CRUDActions.Update;
    }

    this.loading = true;
    result.subscribe({
      next: (response: any) => {
        if(response.actionSuccess == true) {
          if(action === CRUDActions.Update) {
            this.reportSchedule!.scheduleDate = reportScheduleUpdate.scheduleDate;
            this.reportSchedule!.isActive = reportScheduleUpdate.isActive!;
          }
          
          this.reportScheduleSubmitted.emit({ success: true, data: { message: response.actionData, action: action, reportSchedule: this.reportSchedule } });
          this.loading = false;
        }
        else{
          this.reportScheduleSubmitted.emit({ success: false, data: { message: response?.errors[0] } });
          this.loading = false;
        }
      },
      error: (error: HttpErrorResponse) => {
        this.reportScheduleSubmitted.emit({ success: false, data: { message: error?.error[0] } });
        this.loading = false;
      }
    });
  }
}
