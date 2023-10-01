import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LoginResponse } from 'src/app/shared/models/login-response';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth.service';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent {

  loginForm!: FormGroup;
  loggingIn: boolean = false;
  loginError: boolean = false;
  loginErrorMessage!: string;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private storageService: StorageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]]
      }
    );
  }

  onLogin(): void {
    if(this.loginForm.invalid){
      return;
    }

    this.loginForm.get('email')?.disable();
    this.loginForm.get('password')?.disable();
    this.loggingIn = true;
    this.loginError = false;
    this.loginErrorMessage = '';

    this.authService.loginUser(this.loginForm.value).subscribe({
        next: (actionResultResponse: ActionResultResponse<LoginResponse>) => {
          if (!actionResultResponse){
            this.loggingIn = false;
            this.loginError = true;
            this.loginForm.get('email')?.enable();
            this.loginForm.get('password')?.enable();
            this.loginErrorMessage = "Unknown error occurred!";
            return;
          }

          if(!actionResultResponse.actionSuccess || actionResultResponse.actionData == null){
            this.loggingIn = false;
            this.loginError = true;
            this.loginForm.get('email')?.enable();
            this.loginForm.get('password')?.enable();
            this.loginErrorMessage = actionResultResponse.errors[0];
            return;
          }

          let loginResponse: LoginResponse = actionResultResponse.actionData;
          this.storageService.storeToken(loginResponse.token);
          this.storageService.storeUserData(loginResponse.userData);

          this.loggingIn = false;
          let url = this.router.routerState.snapshot.root.queryParams["redirectTo"];

          if(url == "" || url == null || url == undefined) {
            this.router.navigate([""]);
          }
          else {
            this.router.navigateByUrl(url);
          }
        }
      }
    );
  }
}