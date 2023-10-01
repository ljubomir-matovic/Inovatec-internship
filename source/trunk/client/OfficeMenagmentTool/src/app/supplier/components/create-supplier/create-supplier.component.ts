import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { OfficeService } from 'src/app/office/services/office.service';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { Office } from 'src/app/shared/models/office';
import { Supplier } from 'src/app/shared/models/supplier';
import { SupplierService } from '../../services/supplier.service';

@Component({
  selector: 'app-create-supplier',
  templateUrl: './create-supplier.component.html',
  styleUrls: ['./create-supplier.component.scss']
})
export class CreateSupplierComponent {
  supplier: Supplier | null = null;
  supplierForm!: FormGroup;
  loading: boolean = false;
  formSubmitted!: EventEmitter<ActionResult<any>>;

  offices!: Office[]

  constructor(
    private formBuilder: FormBuilder,
    private supplierService: SupplierService,
    private dialogConfig: DynamicDialogConfig
  ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.supplier !== undefined) {
      this.supplier = this.dialogConfig.data.supplier;
    }

    this.supplierForm = this.formBuilder.group({
      name: [this.supplier?.name, Validators.required],
      phoneNumber: [this.supplier?.phoneNumber, Validators.required],
      country: [this.supplier?.country, Validators.required],
      city: [this.supplier?.city, Validators.required],
      address: [this.supplier?.address, Validators.required]
    });

    this.formSubmitted = this.dialogConfig.data.formSubmitted;
  }

  submitForm() {
    let result;
    let action:CRUDActions;

    if(this.supplier == null){
      result = this.supplierService.addSupplier(this.supplierForm.value);
      action = CRUDActions.Create;
    }
    else{
      result=this.supplierService.updateSupplier({ ...this.supplier, ...this.supplierForm.value });
      action = CRUDActions.Update;
    }

    this.loading = true;
    result.subscribe({
      next:(message: any) => {
        if(action == CRUDActions.Update) {
          this.supplier!.name = this.supplierForm.value.name;
          this.supplier!.phoneNumber = this.supplierForm.value.phoneNumber;
          this.supplier!.country = this.supplierForm.value.country;
          this.supplier!.city = this.supplierForm.value.city;
          this.supplier!.address = this.supplierForm.value.address;
        }
        this.formSubmitted.emit({ success: true, data: { message: message.actionData, action, supplier: this.supplier } });
        this.loading = false;
      },
      error: (error: HttpErrorResponse) => {
        this.formSubmitted.emit({ success: false, data: { message: error?.error[0] } });
        this.loading = false;
      }
    });
  }
}
