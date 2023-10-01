import { ChangeDetectorRef, Component, EventEmitter, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { Order } from 'src/app/shared/models/order';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { OfficeService } from 'src/app/office/services/office.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Office } from 'src/app/shared/models/office';
import { HttpErrorResponse } from '@angular/common/http';
import { OrderRequestService } from '../../services/order-request.service';
import { SnackOrderUpdate } from 'src/app/shared/models/snack-order-update';

@Component({
  selector: 'app-update-snack-order',
  templateUrl: './update-snack-order.component.html',
  styleUrls: ['./update-snack-order.component.scss']
})
export class UpdateSnackOrderComponent {
  order!: Order;
  orderUpdateForm!: FormGroup;
  sending: boolean = false;
  
  formSubmitted!:EventEmitter<ActionResult<any>>;

  offices: Office[] = [];

  get officeId(): AbstractControl {
    return this.orderUpdateForm.controls['officeId'];
  }

  get amount(): AbstractControl {
    return this.orderUpdateForm.controls['amount'];
  }

  constructor(
    private formBuilder: FormBuilder, 
    private orderRequestService: OrderRequestService, 
    private dialogConfig: DynamicDialogConfig,
    private officeService: OfficeService,
    private changeDetectorRef: ChangeDetectorRef
  ) { }

  getOffices(): void {
    let officeFilter: OfficeFilter = {
      PageNumber: 0,
      PageSize: 0,
      SortField: 'name',
      SortOrder: 1
    };

    this.officeService.getOffices(officeFilter).subscribe(
      {
        next: (result: ActionResultResponse<DataPage<Office>>) => {
          if(!result.actionSuccess || result.actionData == null) {
            this.offices = [];
          }
          this.offices = result.actionData.data;
          this.changeDetectorRef.detectChanges();
        },
        error: (error: any) => {
          this.offices = [];
        }
      }
    );
  }

  ngOnInit(): void {
    if(this.dialogConfig.data?.order !== undefined) {
      this.order = this.dialogConfig.data.order;
    }
    
    this.orderUpdateForm = this.formBuilder.group({
      orderId: [this.order.id],
      officeId: [this.order.office.id],
      amount: [this.order.amount, Validators.required]
    });

    this.formSubmitted = this.dialogConfig.data.formSubmitted;
    this.getOffices();
  }

  submitForm() {
    if(!this.orderUpdateForm.valid) {
      return;
    }

    let orderUpdateData: SnackOrderUpdate = this.orderUpdateForm.value;

    this.sending = true;
    this.orderRequestService.updateSnackOrder(orderUpdateData).subscribe({
      next: (msg: any) => {
        this.order.amount = orderUpdateData.amount;
        this.order.office = this.offices.find(office => office.id === orderUpdateData.officeId)!;
        this.formSubmitted.emit({ success: true, data: { message: msg.actionData, order: this.order } });
        this.sending = false;
      },
      error: (error: HttpErrorResponse) => {
        this.formSubmitted.emit({ success: false, data: { message: error?.error[0] } });
        this.sending = false;
      }
    });
  }
}
