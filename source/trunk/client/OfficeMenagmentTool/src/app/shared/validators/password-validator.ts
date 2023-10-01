import { AbstractControl, ValidatorFn, Validators } from "@angular/forms";

export class PasswordValidator{

  static get validatePassword(): ValidatorFn {
    let validatePassword:ValidatorFn = (control:AbstractControl) => {
      if(control.value == '' || control.value == undefined) {
        return { required: true };
      }
      return Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\-_])[A-Za-z\d@$!%*?&\-_]{8,}$/)(control);
    };
    return validatePassword;
  }

  static validateConfirmedPassword(password:AbstractControl): ValidatorFn {
    let passwordConfirmedValidator:ValidatorFn = (control:AbstractControl) => {
      if(control.value == '' || control.value == undefined) {
        return { required: true };
      }
      if(password.value == null || password.value == control.value) {
        return null;
      }
      return { confirmedPassword: true };
    };

    return passwordConfirmedValidator;
  }
}
