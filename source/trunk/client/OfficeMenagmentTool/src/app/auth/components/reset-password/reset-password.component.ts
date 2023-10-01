import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { AuthService } from 'src/app/shared/services/auth.service';
import { PasswordValidator } from 'src/app/shared/validators/password-validator';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  sending:boolean = false;
  token!:string;
  modalVisible:boolean = false;
  public myForm!:FormGroup;

  get password(): AbstractControl {
    return this.myForm.controls['password'];
  }

  get confirmedPassword(): AbstractControl {
    return this.myForm.controls['confirmedPassword'];
  }

  get passwordIsInvalid(): boolean {
    return !this.password.valid && this.password.dirty;
  }

  get confirmedPasswordIsInvalid(): boolean {
    return !this.confirmedPassword.valid && this.confirmedPassword.touched;
  }

  constructor(
    private activatedRoute:ActivatedRoute,
    private fb:FormBuilder,
    private authService:AuthService,
    private messageService:MessageService,
    private router:Router){}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      this.token = params['token'] ?? "aaa";
    });

    this.myForm = this.fb.group({
      password: ['', PasswordValidator.validatePassword],
      confirmedPassword: ['']
    });
    this.confirmedPassword.addValidators(PasswordValidator.validateConfirmedPassword(this.password));
    this.confirmedPassword.updateValueAndValidity();
  }

  submit() {
    this.sending = true;
    this.authService.resetPassword(this.token, this.password.value).subscribe({
      next:(response:any) => {
        this.sending = false;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Password changed successfully.' });
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'You will be redirected to login page now.' });
        setTimeout(() => {
          this.router.navigateByUrl("/");
        },5000);
      },
      error:(error:HttpErrorResponse) => {
        this.sending = false;
        this.messageService.add({ severity: 'error', summary: 'Error', detail: error?.error[0] });
      }
    })
  }
}
