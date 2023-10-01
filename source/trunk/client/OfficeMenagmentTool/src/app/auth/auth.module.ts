import { NgModule } from '@angular/core';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { SharedModule } from '../shared/shared.module';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { LoginComponent } from './components/login/login.component';

@NgModule({
  declarations: [
    ResetPasswordComponent,
    ForgotPasswordComponent,
    LoginComponent
  ],
  imports: [
    SharedModule
  ]
})
export class AuthModule { }
