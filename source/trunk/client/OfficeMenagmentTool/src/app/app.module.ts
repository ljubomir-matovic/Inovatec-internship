import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LayoutComponent } from './layout/layout.component';
import { AdminModule } from './admin/admin.module';
import { AuthModule } from './auth/auth.module';
import { SharedModule } from './shared/shared.module';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { JwtInterceptor } from './shared/interceptors/jwt.interceptor';
import { StorageService } from './shared/helpers/storage.service';
import { Router } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { ProductsModule } from './products/products.module';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { environment } from 'src/environments/environment';
import { OrderModule } from './order/order.module';
import { PersonalModule } from './personal/personal.module';
import { OfficeModule } from './office/office.module';
import { ReportScheduleModule } from './report-schedule/report-schedule.module';
import { SupplierModule } from './supplier/supplier.module';
export function httpTranslateLoader(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http,`${environment.serverUrl}/api/Translate/`,"");
}

@NgModule({
  declarations: [
    AppComponent,
    LayoutComponent,
    HeaderComponent
  ],
  imports: [
    AppRoutingModule,
    SharedModule,
    AdminModule,
    AuthModule,
    OrderModule,
    ProductsModule,
    PersonalModule,
    OfficeModule,
    ReportScheduleModule,
    SupplierModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: httpTranslateLoader,
        deps: [HttpClient]
      }
    })
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true,
      deps: [
        Router,
        StorageService
      ]
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
