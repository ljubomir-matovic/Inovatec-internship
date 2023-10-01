import { NgModule } from '@angular/core';
import { ReportSchedulesAdministrationComponent } from './components/report-schedules-administration/report-schedules-administration.component';
import { SharedModule } from '../shared/shared.module';
import { CreateReportScheduleComponent } from './components/create-report-schedule/create-report-schedule.component';

@NgModule({
  declarations: [
    ReportSchedulesAdministrationComponent,
    CreateReportScheduleComponent
  ],
  imports: [
    SharedModule
  ]
})
export class ReportScheduleModule { }
