import { NgModule } from '@angular/core';
import { SupplierAdministrationComponent } from './components/supplier-administration/supplier-administration.component';
import { CreateSupplierComponent } from './components/create-supplier/create-supplier.component';
import { SupplierService } from './services/supplier.service';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [
    SupplierAdministrationComponent,
    CreateSupplierComponent
  ],
  imports: [
    SharedModule
  ],
  providers:  [
    SupplierService
  ]
})
export class SupplierModule { }
