import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { User } from 'src/app/shared/models/user.model';
import { UserService } from '../../services/user.service';
import { ROLES } from 'src/app/shared/constants/role';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { HttpErrorResponse } from '@angular/common/http';
import { Office } from 'src/app/shared/models/office';
import { OfficeService } from 'src/app/office/services/office.service';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Dropdown } from 'primeng/dropdown';

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss']
})
export class CreateUserComponent implements OnInit {
  @ViewChild("officesDropdown") officesDropdown!: Dropdown;

  user: User | null = null;
  myForm!: FormGroup;
  sending: boolean = false;
  formSubmitted!: EventEmitter<ActionResult<any>>;
  public roles = ROLES;

  offices!: Office[]

  get firstName(): AbstractControl {
    return this.myForm.controls['firstName'];
  };
  get lastName(): AbstractControl {
    return this.myForm.controls['lastName'];
  };
  get email(): AbstractControl {
    return this.myForm.controls['email'];
  };
  get role(): AbstractControl {
    return this.myForm.controls['role'];
  };
  get officeId(): AbstractControl {
    return this.myForm.controls['officeId'];
  };

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private dialogConfig: DynamicDialogConfig,
    private officeService: OfficeService
  ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.user !== undefined) {
      this.user = this.dialogConfig.data.user;
    }

    this.loadOffices();

    this.myForm = this.fb.group({
      firstName: [this.user?.firstName, Validators.required],
      lastName: [this.user?.lastName, Validators.required ],
      email: [this.user?.email, Validators.compose([Validators.email, Validators.required])],
      role: [this.user?.role, Validators.required],
      officeId: [this.user?.officeId, Validators.required]
    });

    this.formSubmitted = this.dialogConfig.data.formSubmitted;

    if(this.user?.role == undefined) {
      this.role.setValue(this.roles[0].id);
    }
  }

  loadOffices(): void {
    let officeFilter: OfficeFilter = {
      PageNumber: 0,
      PageSize: 0,
      SortField: 'name',
      SortOrder: 1
    }

    this.offices = [];
    this.officeService.getOffices(officeFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Office>>) => {
        if(response.actionSuccess == true) {
          this.offices = response.actionData.data;
          if(this.user?.officeId == undefined) {
            this.officeId.setValue(this.offices[0].id);
          }
        }
        else{
          this.offices = [];
        }
      },
      error: (error: HttpErrorResponse) => {
        this.offices = [];
      }
    });
  }

  submitForm() {
    let result;
    let action:CRUDActions;
    if(this.user == null){
      result = this.userService.createUser(this.myForm.value);
      action = CRUDActions.Create;
    }else{
      result=this.userService.updateUser({ ...this.user, ...this.myForm.value });
      action = CRUDActions.Update;
    }
    this.sending = true;
    result.subscribe({
      next:(msg:any) => {
        this.formSubmitted.emit({ success: true, data: { message:msg.actionData, action } });
        this.sending = false;
      },
      error: (error: HttpErrorResponse) => {
        this.formSubmitted.emit({ success: false, data: { message: error?.error[0] } });
        this.sending = false;
      }
    });
  }
}
