import { Component, EventEmitter, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, LazyLoadEvent, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { EquipmentService } from '../../services/equipment.service';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { EquipmentFilter } from 'src/app/shared/models/equipment-filter.model';
import { CreateEquipmentComponent } from '../create-equipment/create-equipment.component';
import { Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-equipment-unassigned',
  templateUrl: './equipment-unassigned.component.html',
  styleUrls: ['./equipment-unassigned.component.scss']
})
export class EquipmentUnassignedComponent implements OnInit {
  equipments!: any[];
  lastLoadEvent!: LazyLoadEvent;
  totalRecords: number = 1;
  loading: boolean = true;
  ref!: DynamicDialogRef;
  dialogStatus!: EventEmitter<any>;
  first: number = 0;
  rows: number = 10;
  position: string = "";
  categories: any[] = [];
  items: any[] = [];
  selectedEquipments!:any[];

  subscriptions: any[] = [];
  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();

  readonly toastKey: string = "toastUnassigned";

  constructor(
    private dialogService:DialogService,
    private messageService:MessageService,
    private confirmationService:ConfirmationService,
    private translateService:TranslateService,
    private equipmentService:EquipmentService,
    ) {}

  ngOnInit(): void {
    this.initSubscriptions();
    this.ref=new DynamicDialogRef();
    this.dialogStatus=new EventEmitter<any>();
    this.setCategories();
    this.setItems();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  getPageNumber(first:number,rows:number): number {
    return ( first / rows ) + 1;
  }

  initSubscriptions(): void {
    this.subscriptions.push(this.onDialogClose);
    this.subscriptions.push(this.onAssign());
    this.subscriptions.push(this.onRefresh());

    this.onDialogClose.subscribe({
      next: (status: ActionResult<any>) => {
        this.dialogClosed(status);
      }
    });
  }

  onAssign(): Subscription {
    return this.equipmentService.onAssign.subscribe(_ => {
      this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
        if(this.selectedEquipments == null || this.selectedEquipments.length == 0) {
          this.messageService.add({severity:"error",summary:translations['Common']['Error'],detail:translations['Equipment']['SelectRow'],key:this.toastKey});
          return;
        }
        this.equipmentService.assignLoading = true;
        this.equipmentService.assign(
          this.selectedEquipments,
          Number(this.equipmentService.selectedUserId)).subscribe({
            next: _ => {
              this.equipmentService.assignLoading = false;
              this.equipmentService.onAssigned.emit();
              this.selectedEquipments = [];
              this.refreshEquipment(CRUDActions.Delete);
            },
            error: (error:HttpErrorResponse) => {
              this.equipmentService.assignLoading = false;
            }
          });
      });
    });
  }

  onRefresh(): Subscription {
    return this.equipmentService.onUnassigned.subscribe(()=>this.refreshEquipment(CRUDActions.Create));
  }

  dialogClosed(status: ActionResult<any>): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      if(status.success) {
        this.refreshEquipment(status.data.action);
        this.ref.close();
        this.messageService.add({ severity: "success", summary: translations['Common']["Success"], detail: translations['Equipment'][status.data.message], key: this.toastKey });
      } else {
        //this.ref.close();
        this.messageService.add({ severity: "error", summary: translations["Common"]["Error"], detail: translations['Equipment'][status.data.message], key: this.toastKey });
      }
    });
  }

  loadEquipment(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || event.rows === undefined)
      return;
    let filter:EquipmentFilter = {
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? "",
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1,
      PageSize: event.rows ?? 10,
      UserId: -1
    };

    if(event.filters !== undefined) {
      filter.ItemId = event.filters["itemId"]?.value ?? 0;
      filter.CategoryId = event.filters["categoryId"]?.value ?? 0;
    }

    this.equipmentService.getEquipments(filter).subscribe({
        next:(value:DataPage<any>) => {
          this.totalRecords = value.totalRecords;
          this.equipments = value.data;
          this.loading = false;
        },
        error:(err:any) => {
          this.loading = false;
          this.equipments = [];
          this.first = 0;
        }
      });
  }

  refreshEquipment(action:CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.loadEquipment({ ...this.lastLoadEvent, first:this.first, rows:this.rows});
        break;
      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.rows);

        if((pageNumber - 1) * this.rows == this.totalRecords - 1 && pageNumber > 1) {
          this.loadEquipment({ ...this.lastLoadEvent, first: first-this.rows, rows: this.rows });
        } else {
          this.loadEquipment({ ...this.lastLoadEvent, first: this.first, rows: this.rows });
        }

        break;
    }
  }

  showAddEquipment(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.ref=this.dialogService.open(CreateEquipmentComponent,{
        header:translations['Equipment']['AddEquipmentLabel'],
        maximizable: true,
        data:{
          formSubmitted:this.onDialogClose
        }
      });
    });
  }

  showDeleteConfirm(id:number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.confirmationService.confirm({
        message: translations['Equipment']['DeleteEquipmentConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.equipmentService.deleteEquipment(id).subscribe({
            next:(msg) => {
              this.refreshEquipment(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['Equipment'][msg.actionData], key: this.toastKey });
            }
          })
        },
        reject: (type:any) => {
            this.confirmationService.close();
        }
      });
    });
  }

  showDeleteSelectedConfirm(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.confirmationService.confirm({
        message: translations['Equipment']['DeleteEquipmentSelectedConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.equipmentService.deleteSelectedEquipments(this.selectedEquipments).subscribe({
            next:(msg:any) => {
              this.selectedEquipments = [];
              this.refreshEquipment(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['Equipment'][msg.actionData], key: this.toastKey });
            }
          })
        },
        reject: (type:any) => {
            this.confirmationService.close();
        }
      });
    });
  }

  clearFilters(table:Table): void {
    table.clear();
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
