import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { AuthService } from 'src/app/shared/services/auth.service';
import { EmailValidator } from 'src/app/shared/validators/email-validator';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {
  myForm!:FormGroup;
  sending:boolean = false;

  get email(): AbstractControl {
    return this.myForm.controls['email'];
  }

  constructor(
    private fb:FormBuilder,
    private authService:AuthService,
    private messageService:MessageService,
    private router:Router
    ) {}

  ngOnInit(): void {
    this.myForm = this.fb.group({
      email: ['', EmailValidator.validateEmail]
    });
  }

  submit(){
    this.sending = true;
    this.authService.forgotPassword(this.email.value).subscribe({
      next:(response:any) => {
        this.sending = false;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Check your email.' });
        setTimeout(() => {
          this.router.navigateByUrl("/");
        },5000);
      },
      error:(error:HttpErrorResponse) => {
        this.sending = false;
        let message = "User with this email doesn't exist.";

        if(error.error.includes("TokenAlreadyCreated")) {
          message = "Token is already created for this account.";
        }

        this.messageService.add({ severity: 'error', summary: 'Error', detail: message });
      }
    });
  }
}
