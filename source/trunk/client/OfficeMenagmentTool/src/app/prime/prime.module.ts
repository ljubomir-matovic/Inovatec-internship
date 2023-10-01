import { NgModule } from '@angular/core';
import { PanelMenuModule } from 'primeng/panelmenu';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputTextModule } from 'primeng/inputtext';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CheckboxModule } from 'primeng/checkbox';
import { MessagesModule } from 'primeng/messages';
import { ProgressBarModule } from 'primeng/progressbar';
import { PasswordModule } from 'primeng/password';
import { DividerModule } from 'primeng/divider';
import { DropdownModule } from 'primeng/dropdown';
import { ListboxModule } from 'primeng/listbox';
import { SelectButtonModule } from 'primeng/selectbutton';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DialogModule } from 'primeng/dialog';
import { ChipModule } from 'primeng/chip';
import { DataViewModule } from 'primeng/dataview';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { SkeletonModule } from 'primeng/skeleton';
import { TabMenuModule } from 'primeng/tabmenu';
import { InputNumberModule } from 'primeng/inputnumber';
import { TabViewModule } from 'primeng/tabview';
import { SplitterModule } from 'primeng/splitter';
import { MenuModule } from 'primeng/menu';
import { FileUploadModule } from 'primeng/fileupload';
import { PanelModule } from 'primeng/panel';
import { InplaceModule } from 'primeng/inplace';
import { BadgeModule } from 'primeng/badge';
import { CalendarModule } from 'primeng/calendar';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { OverlayModule } from 'primeng/overlay';
import { SidebarModule } from 'primeng/sidebar';

@NgModule({
  declarations: [],
  providers:[
    DialogService,
    MessageService,
    ConfirmationService
  ],
  imports: [],
  exports: [
    PanelMenuModule,
    TableModule,
    MultiSelectModule,
    InputTextModule,
    DynamicDialogModule,
    ToastModule,
    ConfirmDialogModule,
    DropdownModule,
    ButtonModule,
    PasswordModule,
    CheckboxModule,
    MessagesModule,
    ProgressBarModule,
    DividerModule,
    ListboxModule,
    SelectButtonModule,
    InputTextareaModule,
    DialogModule,
    ChipModule,
    DataViewModule,
    VirtualScrollerModule,
    SkeletonModule,
    TabMenuModule,
    InputNumberModule,
    TabViewModule,
    SplitterModule,
    MenuModule,
    FileUploadModule,
    PanelModule,
    InplaceModule,
    BadgeModule,
    CalendarModule,
    ToggleButtonModule,
    OverlayModule,
    SidebarModule
  ]
})
export class PrimeModule { }
