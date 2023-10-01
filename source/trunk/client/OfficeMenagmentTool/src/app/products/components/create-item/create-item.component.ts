import { Component, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { Category } from 'src/app/shared/models/category';
import { Item } from 'src/app/shared/models/item';
import { CategoriesService } from '../../services/categories.service';
import { ItemService } from '../../services/item.service';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-create-item',
  templateUrl: './create-item.component.html',
  styleUrls: ['./create-item.component.scss']
})
export class CreateItemComponent {
  item: Item | null = null;
  itemForm!: FormGroup;
  itemSubmitted!: EventEmitter<ActionResult<any>>;
  itemCategories!: Category[]
  loading: boolean = false;

  get name(): AbstractControl {
    return this.itemForm.controls['name'];
  };

  get category(): AbstractControl {
    return this.itemForm.controls['categoryId'];
  };

  constructor(
    private formBuilder: FormBuilder, 
    private dialogConfig: DynamicDialogConfig, 
    private categoryService: CategoriesService,
    private itemService: ItemService
  ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.item !== undefined) {
      this.item = this.dialogConfig.data.item;
    }

    this.itemForm = this.formBuilder.group({
      name: [this.item?.name, Validators.required],
      categoryId: [this.item?.category, Validators.required]
    });

    this.itemSubmitted = this.dialogConfig.data.itemSubmitted;

    this.itemCategories = this.dialogConfig.data?.categories;

    if(this.item?.category == undefined){
      this.category.setValue(this.itemCategories[0].id);
    }
  }

  submitItem(): void {
    let result;
    let action: CRUDActions;

    if(this.item == null) {
      result = this.itemService.createItem(this.itemForm.value);
      action = CRUDActions.Create;
    }
    else {
      result = this.itemService.updateItem({ ...this.item, ...this.itemForm.value });
      action = CRUDActions.Update;
    }

    this.loading = true;
    result.subscribe({
      next: (response: any) => {
        if(response.actionSuccess == true) {
          this.itemSubmitted.emit({ success: true, data: { message: response.actionData, actions: action } });
          this.loading = false;
        }
        else{
          this.itemSubmitted.emit({ success: false, data: { message: response?.errors[0] } });
          this.loading = false;
        }
      },
      error: (error: HttpErrorResponse) => {
        this.itemSubmitted.emit({ success: false, data: { message: error?.error[0] } });
        this.loading = false;
      }
    });
  }
}