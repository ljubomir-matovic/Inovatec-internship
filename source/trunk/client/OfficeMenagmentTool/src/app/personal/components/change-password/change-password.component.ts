import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { PasswordValidator } from 'src/app/shared/validators/password-validator';
import { UserService } from '../../../admin/services/user.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  sending:boolean = false;
  public changePasswordForm!:FormGroup;

  get oldPassword(): AbstractControl {
    return this.changePasswordForm.controls['oldPassword'];
  }

  get newPassword(): AbstractControl {
    return this.changePasswordForm.controls['newPassword'];
  }

  get confirmedPassword(): AbstractControl {
    return this.changePasswordForm.controls['confirmedPassword'];
  }

  get oldPasswordIsInvalid(): boolean {
    return !this.oldPassword.valid && this.oldPassword.dirty;
  }

  get newPasswordIsInvalid(): boolean {
    return !this.newPassword.valid && this.newPassword.dirty;
  }

  get confirmedPasswordIsInvalid(): boolean {
    return !this.confirmedPassword.valid && this.confirmedPassword.touched;
  }

  constructor(
    private fb:FormBuilder,
    private userService:UserService,
    private messageService:MessageService,
    private translateService: TranslateService){}

  ngOnInit(): void {
    this.changePasswordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', PasswordValidator.validatePassword],
      confirmedPassword: ['']
    });
    this.confirmedPassword.addValidators(PasswordValidator.validateConfirmedPassword(this.newPassword));
    this.confirmedPassword.updateValueAndValidity();
  }

  submitForm() {
    this.sending = true;
    this.userService.changePassword(this.oldPassword.value, this.newPassword.value).subscribe({
      next:(response: any) => {
        this.sending = false;
        if(response?.actionData) {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: this.translateService.instant(response.actionData) });
        }
        else {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: this.translateService.instant(response.actionData) });
        }
      },
      error:(error: HttpErrorResponse) => {
        this.sending = false;
        if(error?.error?.length > 0) {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: this.translateService.instant(error?.error[0]) });
        }
      }
    })
  }
}
