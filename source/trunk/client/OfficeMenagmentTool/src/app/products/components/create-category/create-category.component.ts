import { Component, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CATEGORY_TYPES } from 'src/app/shared/constants/category-types';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { Category } from 'src/app/shared/models/category';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { CategoriesService } from '../../services/categories.service';
import { HttpErrorResponse } from '@angular/common/http';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';

@Component({
  selector: 'app-create-category',
  templateUrl: './create-category.component.html',
  styleUrls: ['./create-category.component.scss']
})
export class CreateCategoryComponent {
  category: Category | null = null;
  categoryForm!: FormGroup;
  categorySubmitted!: EventEmitter<ActionResult<any>>;
  categoryTypes = CATEGORY_TYPES;
  loading: boolean = false;

  get name(): AbstractControl {
    return this.categoryForm.controls['name'];
  };

  get type(): AbstractControl {
    return this.categoryForm.controls['type'];
  };

  constructor(
    private formBuilder: FormBuilder, 
    private dialogConfig: DynamicDialogConfig, 
    private categoryService: CategoriesService
  ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.category !== undefined) {
      this.category = this.dialogConfig.data.category;
    }

    this.categoryForm = this.formBuilder.group({
      name: [this.category?.name, Validators.required],
      type: [this.category?.type, Validators.required]
    });

    this.categorySubmitted = this.dialogConfig.data.categorySubmitted;

    if(this.category?.type == undefined){
      this.type.setValue(this.categoryTypes[0].id);
    }
  }

  submitCategory(): void {
    let result;
    let action: CRUDActions;
    
    if(this.category == null) {
      result = this.categoryService.createCategory(this.categoryForm.value);
      action = CRUDActions.Create;
    }
    else {
      result = this.categoryService.updateCategory({ ...this.category, ...this.categoryForm.value });
      action = CRUDActions.Update;
    }

    this.loading = true;
    result.subscribe({
      next: (response: any) => {
        if(response.actionSuccess == true) {
          this.categorySubmitted.emit({ success: true, data: { message: response.actionData, actions: action } });
          this.loading = false;
        }
        else{
          this.categorySubmitted.emit({ success: false, data: { message: response?.errors[0] } });
          this.loading = false;
        }
      },
      error: (error: HttpErrorResponse) => {
        this.categorySubmitted.emit({ success: false, data: { message: error?.error[0] } });
        this.loading = false;
      }
    });
  }
}