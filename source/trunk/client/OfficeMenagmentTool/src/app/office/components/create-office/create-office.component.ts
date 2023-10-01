import { Component, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { Office } from 'src/app/shared/models/office';
import { OfficeService } from '../../services/office.service';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-create-office',
  templateUrl: './create-office.component.html',
  styleUrls: ['./create-office.component.scss']
})
export class CreateOfficeComponent {
  office: Office | null = null;
  officeForm!: FormGroup;
  officeSubmitted!: EventEmitter<ActionResult<any>>;
  loading: boolean = false;

  get name(): AbstractControl {
    return this.officeForm.controls['name'];
  };

  constructor(
    private formBuilder: FormBuilder, 
    private dialogConfig: DynamicDialogConfig, 
    private officeService: OfficeService
  ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.office !== undefined) {
      this.office = this.dialogConfig.data.office;
    }

    this.officeForm = this.formBuilder.group({
      name: [this.office?.name, Validators.required]
    });

    this.officeSubmitted = this.dialogConfig.data.officeSubmitted;
  }

  submitOffice(): void {
    let result;
    let action: CRUDActions;
    
    if(this.office == null) {
      result = this.officeService.addOffice(this.officeForm.value);
      action = CRUDActions.Create;
    }
    else {
      result = this.officeService.updateOffice({ ...this.office, ...this.officeForm.value });
      action = CRUDActions.Update;
    }

    this.loading = true;
    result.subscribe({
      next: (response: any) => {
        if(response.actionSuccess == true) {
          if(action === CRUDActions.Update) {
            this.office!.name = this.officeForm.value.name;
          }
          this.officeSubmitted.emit({ success: true, data: { message: response.actionData, action: action, office: this.officeForm.value.name } });
          this.loading = false;
        }
        else{
          this.officeSubmitted.emit({ success: false, data: { message: response?.errors[0] } });
          this.loading = false;
        }
      },
      error: (error: HttpErrorResponse) => {
        this.officeSubmitted.emit({ success: false, data: { message: error?.error[0] } });
        this.loading = false;
      }
    });
  }
}
