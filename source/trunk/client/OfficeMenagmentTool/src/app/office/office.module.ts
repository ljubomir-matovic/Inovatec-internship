import { NgModule } from '@angular/core';
import { CreateOfficeComponent } from './components/create-office/create-office.component';
import { OfficeService } from './services/office.service';
import { OfficeAdministrationComponent } from './components/office-administration/office-administration.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    OfficeAdministrationComponent,
    CreateOfficeComponent
  ],
  imports: [
    SharedModule
  ],
  providers:  [
    OfficeService
  ]
})
export class OfficeModule { }