import { NgModule } from '@angular/core';
import { ProductsAdministrationComponent } from './components/products-administration/products-administration.component';
import { CategoriesAdministrationComponent } from './components/categories-administration/categories-administration.component';
import { SharedModule } from '../shared/shared.module';
import { CreateCategoryComponent } from './components/create-category/create-category.component';
import { CreateItemComponent } from './components/create-item/create-item.component';
import { EquipmentEvidentionComponent } from './components/equipment-evidention/equipment-evidention.component';
import { CreateEquipmentComponent } from './components/create-equipment/create-equipment.component';
import { EquipmentUnassignedComponent } from './components/equipment-unassigned/equipment-unassigned.component';
import { EquipmentAssignedComponent } from './components/equipment-assigned/equipment-assigned.component';
import { OrderedSnacksComponent } from '../order/components/ordered-snacks/ordered-snacks.component';
import { EquipmentService } from './services/equipment.service';

@NgModule({
  declarations: [
    ProductsAdministrationComponent,
    CategoriesAdministrationComponent,
    CreateCategoryComponent,
    CreateItemComponent,
    EquipmentEvidentionComponent,
    CreateEquipmentComponent,
    EquipmentUnassignedComponent,
    EquipmentAssignedComponent,
    OrderedSnacksComponent
  ],
  imports: [
    SharedModule
  ],
  providers: [
    EquipmentService
  ]
})
export class ProductsModule { }
