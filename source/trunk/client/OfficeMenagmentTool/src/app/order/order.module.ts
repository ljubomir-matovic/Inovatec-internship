import { NgModule } from '@angular/core';
import { ReportProblemComponent } from './components/report-problem/report-problem.component';
import { SharedModule } from '../shared/shared.module';
import { ReportsAdministrationComponent } from './components/reports-administration/reports-administration.component';
import { OpenReportComponent } from './components/open-report/open-report.component';
import { AcceptOrdersComponent } from './components/accept-orders/accept-orders.component';
import { SnackOrderingComponent } from './components/snack-ordering/snack-ordering.component';
import { OrderEquipmentComponent } from './components/order-equipment/order-equipment.component';
import { EquipmentOrdersAdministrationComponent } from './components/equipment-orders-administration/equipment-orders-administration.component';
import { OpenEquipmentOrderComponent } from './components/open-equipment-order/open-equipment-order.component';
import { OrdersListingComponent } from './components/orders-listing/orders-listing.component';
import { SingleOrderComponent } from './components/single-order/single-order.component';
import { CreateOrderItemComponent } from './components/create-order-item/create-order-item.component';
import { OrderAttachmentComponent } from './components/order-attachment/order-attachment.component';
import { OrderItemsComponent } from './components/order-items/order-items.component';
import { OrderAttachmentsComponent } from './components/order-attachments/order-attachments.component';
import { UpdateSnackOrderComponent } from './components/update-snack-order/update-snack-order.component';

@NgModule({
  declarations: [
    ReportProblemComponent,
    ReportsAdministrationComponent,
    OpenReportComponent,
    SnackOrderingComponent,
    AcceptOrdersComponent,
    OrdersListingComponent,
    SingleOrderComponent,
    CreateOrderItemComponent,
    OrderAttachmentComponent,
    OrderItemsComponent,
    OrderAttachmentsComponent,
    OrderEquipmentComponent,
    EquipmentOrdersAdministrationComponent,
    OpenEquipmentOrderComponent,
    UpdateSnackOrderComponent
  ],
  imports: [
    SharedModule
  ]
})
export class OrderModule { }
