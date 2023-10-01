import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { EquipmentService } from '../../services/equipment.service';

@Component({
  selector: 'app-create-equipment',
  templateUrl: './create-equipment.component.html',
  styleUrls: ['./create-equipment.component.scss']
})
export class CreateEquipmentComponent implements OnInit{
  myForm!:FormGroup;
  sending:boolean = false;
  formSubmitted!:EventEmitter<ActionResult<any>>;
  categories:any[] = [];
  items:any[] = [];

  get item(): AbstractControl {
    return this.myForm.controls['itemId'];
  };

  constructor(private fb:FormBuilder, private equipmentService:EquipmentService, private dialogConfig: DynamicDialogConfig){}

  ngOnInit(): void {
    this.myForm = this.fb.group({
      itemId:['', Validators.required],
    });
    this.formSubmitted = this.dialogConfig.data.formSubmitted;
    this.setCategories();
    this.setItems();
  }

  submitForm() {
    let result;
    let action:CRUDActions;
    result = this.equipmentService.createEquipment(this.myForm.value);
    action = CRUDActions.Create;
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
    this.equipmentService.getCategories().subscribe(result => {
      if(result.actionSuccess) {
        this.categories = result.actionData.data;
      }
    })
  }

  setItems(category?:any): void {
    this.equipmentService.getItems(category).subscribe(result => {
      if(result.actionSuccess) {
        this.items = result.actionData.data;
      }
    });
  }
}
