import { NgModule } from '@angular/core';
import { EditProfileComponent } from './components/edit-profile/edit-profile.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { SharedModule } from '../shared/shared.module';
import { AdminModule } from '../admin/admin.module';
import { EmployeeEquipmentsComponent } from './components/employee-equipments/employee-equipments.component';
import { ProductsModule } from '../products/products.module';
import { OrdersHistoryComponent } from './components/orders-history/orders-history.component';

@NgModule({
  declarations: [
    EditProfileComponent,
    ChangePasswordComponent,
    EmployeeEquipmentsComponent,
    OrdersHistoryComponent
  ],
  imports: [
    SharedModule,
    AdminModule,
    ProductsModule
  ],
  exports:[]
})
export class PersonalModule{}
