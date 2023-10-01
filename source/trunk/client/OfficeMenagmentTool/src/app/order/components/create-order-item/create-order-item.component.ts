import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { OrderItemService } from '../../services/order-item.service';

@Component({
  selector: 'app-create-order-item',
  templateUrl: './create-order-item.component.html',
  styleUrls: ['./create-order-item.component.scss']
})
export class CreateOrderItemComponent implements OnInit{
  orderId!: number;
  orderItem: any = null;
  myForm!:FormGroup;
  sending:boolean = false;
  formSubmitted!:EventEmitter<ActionResult<any>>;
  categories:any[] = [];
  items:any[] = [];

  get item(): AbstractControl {
    return this.myForm.controls['itemId'];
  }

  get amount(): AbstractControl {
    return this.myForm.controls['amount'];
  }

  constructor(private fb:FormBuilder, private orderItemService: OrderItemService, private dialogConfig: DynamicDialogConfig){}

  ngOnInit(): void {
    if(this.dialogConfig.data?.orderItem !== undefined) {
      this.orderItem = this.dialogConfig.data.orderItem;
    }

    this.orderId = this.dialogConfig.data.orderId;

    this.myForm = this.fb.group({
      itemId:['', Validators.required],
      amount: [0, Validators.required],
      orderId: [this.orderId]
    });
    this.formSubmitted = this.dialogConfig.data.formSubmitted;
    this.setCategories();
    this.setItems();
  }

  submitForm() {
    let result;
    let action:CRUDActions;
    if(this.orderItem == null) {
      result = this.orderItemService.createOrderItem(this.myForm.value);
      action = CRUDActions.Create;
    }
    else {
      result = this.orderItemService.changeAmount(this.orderId, this.orderItem.id, this.amount.value);
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

  setCategories(): void {
    this.orderItemService.getCategories().subscribe(result => {
      if(result.actionSuccess) {
        this.categories = result.actionData;
      }
    })
  }

  setItems(category?:any): void {
    this.orderItemService.getItems(category).subscribe(result => {
      if(result.actionSuccess) {
        this.items = result.actionData;
      }
    });
  }
}
