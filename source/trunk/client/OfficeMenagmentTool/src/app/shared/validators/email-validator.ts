import { AbstractControl, ValidatorFn, Validators } from "@angular/forms";

export class EmailValidator{

  static get validateEmail(): ValidatorFn {

    let validateEmailFunc:ValidatorFn = (control:AbstractControl) => {

      if(control.value == '') {
        return { required: true };
      }

      return Validators.pattern(/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/)(control);
    };

    return validateEmailFunc;
  }
}
