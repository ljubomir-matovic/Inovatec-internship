import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { PrimeModule } from "../prime/prime.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { TranslateModule } from "@ngx-translate/core";
import { ChangeLanguageComponent } from './components/change-language/change-language.component';
import { NotificationMenuComponent } from './components/notification-menu/notification-menu.component';
import { NotificationComponent } from './components/notification/notification.component';
import { NotificationGroupComponent } from './components/notification-group/notification-group.component';
import { BusyLoaderComponent } from './components/busy-loader/busy-loader.component';
import { NgBusyModule } from 'ng-busy'

@NgModule({
  declarations:[
    ChangeLanguageComponent,
    NotificationMenuComponent,
    NotificationComponent,
    NotificationGroupComponent,
    BusyLoaderComponent
  ],
  imports:[
    CommonModule,
    BrowserAnimationsModule,
    PrimeModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    NgBusyModule
  ],
  exports:[
    CommonModule,
    BrowserAnimationsModule,
    PrimeModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    TranslateModule,
    ChangeLanguageComponent,
    NotificationMenuComponent,
    NgBusyModule
  ]
})
export class SharedModule{}
