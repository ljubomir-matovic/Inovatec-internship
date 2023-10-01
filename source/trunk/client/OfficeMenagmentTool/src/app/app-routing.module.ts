import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/components/login/login.component';
import { AnonymousGuard, AuthorizedGuard } from './shared/guards/authentication.guard';
import { AdminDashboardComponent } from './admin/components/admin-dashboard/admin-dashboard.component';
import { ResetPasswordComponent } from './auth/components/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './auth/components/forgot-password/forgot-password.component';
import { LayoutComponent } from './layout/layout.component';
import { Role } from './shared/constants/role';
import { AppRoutes } from './shared/constants/routes';
import { EquipmentEvidentionComponent } from './products/components/equipment-evidention/equipment-evidention.component';
import { CategoryTypes } from './shared/constants/category-types';
import { ProductsAdministrationComponent } from './products/components/products-administration/products-administration.component';
import { CategoriesAdministrationComponent } from './products/components/categories-administration/categories-administration.component';
import { ReportProblemComponent } from './order/components/report-problem/report-problem.component';
import { ReportsAdministrationComponent } from './order/components/reports-administration/reports-administration.component';
import { AcceptOrdersComponent } from './order/components/accept-orders/accept-orders.component';
import { SnackOrderingComponent } from './order/components/snack-ordering/snack-ordering.component';
import { OrderedSnacksComponent } from './order/components/ordered-snacks/ordered-snacks.component';
import { EditProfileComponent } from './personal/components/edit-profile/edit-profile.component';
import { ChangePasswordComponent } from './personal/components/change-password/change-password.component';
import { EmployeeEquipmentsComponent } from './personal/components/employee-equipments/employee-equipments.component';
import { OrdersListingComponent } from './order/components/orders-listing/orders-listing.component';
import { SingleOrderComponent } from './order/components/single-order/single-order.component';
import { OrderEquipmentComponent } from './order/components/order-equipment/order-equipment.component';
import { EquipmentOrdersAdministrationComponent } from './order/components/equipment-orders-administration/equipment-orders-administration.component';
import { LogsAdministrationComponent } from './admin/components/logs-administration/logs-administration.component';
import { OfficeAdministrationComponent } from './office/components/office-administration/office-administration.component';
import { ReportSchedulesAdministrationComponent } from './report-schedule/components/report-schedules-administration/report-schedules-administration.component';
import { SupplierAdministrationComponent } from './supplier/components/supplier-administration/supplier-administration.component';

const routes: Routes = [
  {
    path: "login",
    component: LoginComponent,
    canActivate: [AnonymousGuard]
  },
  {
    path: "forgot-password",
    component: ForgotPasswordComponent,
    canActivate: [AnonymousGuard]
  },
  {
    path: "reset-password",
    component: ResetPasswordComponent,
    canActivate: [AnonymousGuard]
  },
  {
    path: "",
    component: LayoutComponent,
    children:
    [
      {
        path: "",
        pathMatch: "full",
        redirectTo: AppRoutes.admin.defaultRoute
      },
      {
        path: AppRoutes.admin.adminDashboard,
        component: AdminDashboardComponent,
        canActivate: [AuthorizedGuard(Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.admin.logsAdministration,
        component: LogsAdministrationComponent,
        canActivate: [AuthorizedGuard(Role.Admin)]
      },
      {
        path: AppRoutes.shared.officeAdministrationRoute,
        component: OfficeAdministrationComponent,
        canActivate: [AuthorizedGuard(Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.hr.reportSchedulesRoute,
        component: ReportSchedulesAdministrationComponent,
        canActivate: [AuthorizedGuard(Role.HR)]
      },
      {
        path: AppRoutes.hr.suppliersAdministrationRoute,
        component: SupplierAdministrationComponent,
        canActivate: [AuthorizedGuard(Role.HR)]
      },
      {
        path: AppRoutes.shared.categoriesAdministrationRoute,
        component: CategoriesAdministrationComponent,
        canActivate: [AuthorizedGuard(Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.snacksAdministrationRoute,
        component: ProductsAdministrationComponent,
        data: { categoryType: CategoryTypes.Snacks },
        canActivate: [AuthorizedGuard(Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.equipmentAdministrationRoute,
        component: ProductsAdministrationComponent,
        data: { categoryType: CategoryTypes.Equipment },
        canActivate: [AuthorizedGuard(Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.equipmentEvidentionRoute,
        component: EquipmentEvidentionComponent,
        canActivate: [AuthorizedGuard(Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.orderSnacksRoute,
        component: SnackOrderingComponent,
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.snackOrdersRoute,
        component: OrderedSnacksComponent,
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.orderEquipment,
        component: OrderEquipmentComponent,
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.equipmentOrders,
        component: EquipmentOrdersAdministrationComponent,
        data: { forCurrentUserOnly: false },
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.myEquipmentOrders,
        component: EquipmentOrdersAdministrationComponent,
        data: { forCurrentUserOnly: true },
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.reportProblemRoute,
        component: ReportProblemComponent,
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.reportedProblemsRoute,
        component: ReportsAdministrationComponent,
        data: { forCurrentUserOnly: false },
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.shared.myProblemsRoute,
        component: ReportsAdministrationComponent,
        data: { forCurrentUserOnly: true },
        canActivate: [AuthorizedGuard(Role.OrdinaryEmployee, Role.HR, Role.Admin)]
      },
      {
        path: AppRoutes.hr.acceptOrder,
        component: AcceptOrdersComponent,
        canActivate: [AuthorizedGuard(Role.HR)]
      },
      {
        path: AppRoutes.shared.editProfileRoute,
        component: EditProfileComponent,
        canActivate : [AuthorizedGuard(Role.Admin, Role.HR, Role.OrdinaryEmployee)]
      },
      {
        path: AppRoutes.shared.changePasswordRoute,
        component: ChangePasswordComponent,
        canActivate : [AuthorizedGuard(Role.Admin, Role.HR, Role.OrdinaryEmployee)]
      },
      {
        path: AppRoutes.shared.myEquipmentsRoute,
        component: EmployeeEquipmentsComponent,
        canActivate : [AuthorizedGuard(Role.Admin, Role.HR, Role.OrdinaryEmployee)]
      },
      {
        path: AppRoutes.hr.ordersListingRoute,
        component: OrdersListingComponent,
        canActivate: [AuthorizedGuard(Role.HR)]
      },
      {
        path: AppRoutes.hr.orderRoute,
        component: SingleOrderComponent,
        canActivate: [AuthorizedGuard(Role.HR)]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
