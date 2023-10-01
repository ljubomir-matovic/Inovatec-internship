import { NgModule } from '@angular/core';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { CreateUserComponent } from './components/create-user/create-user.component';
import { SharedModule } from '../shared/shared.module';
import { UserService } from './services/user.service';
import { LogsAdministrationComponent } from './components/logs-administration/logs-administration.component';
import { AddCSVComponent } from './components/add-csv/add-csv.component';
import { NgBusyModule } from 'ng-busy';

@NgModule({
  declarations: [
    AdminDashboardComponent,
    CreateUserComponent,
    LogsAdministrationComponent,
    AddCSVComponent
  ],
  imports: [
    SharedModule
  ],
  providers:  [
    UserService
  ]
})
export class AdminModule { }
