import { Component, EventEmitter, OnInit } from '@angular/core';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { EquipmentService } from '../../../products/services/equipment.service';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { EquipmentFilter } from 'src/app/shared/models/equipment-filter.model';
import { UserService } from 'src/app/admin/services/user.service';
import { Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { TranslateService } from '@ngx-translate/core';
import { StorageService } from 'src/app/shared/helpers/storage.service';

@Component({
  selector: 'app-employee-equipments',
  templateUrl: './employee-equipments.component.html',
  styleUrls: ['./employee-equipments.component.scss']
})
export class EmployeeEquipmentsComponent implements OnInit{
  equipments!: any[];
  lastLoadEvent!: LazyLoadEvent;
  totalRecords: number = 1;
  loading: boolean=true;
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

  readonly toastKey: string = "toastAssigned";

  users!:any[];

  constructor(
    private messageService:MessageService,
    private equipmentService:EquipmentService,
    private userService:UserService,
    private translateService:TranslateService,
    private storageService: StorageService
    ) {}

  ngOnInit(): void {
    this.initSubscriptions();
    this.ref=new DynamicDialogRef();
    this.setCategories();
    this.setItems();
    this.userService.getAllUsers({
      PageNumber: 1,
      PageSize: 1000,
      SortOrder: 1
    }).subscribe(result => {
      this.users = result.data;
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  getPageNumber(first:number,rows:number): number {
    return ( first / rows ) + 1;
  }

  initSubscriptions(): void {
    this.subscriptions.push(this.onRefresh());
    //this.subscriptions.push(this.onUnassign());
  }

  onUnassign(): Subscription {
    return this.equipmentService.onUnassign.subscribe(() => {
      this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
        if (this.selectedEquipments == null || this.selectedEquipments.length == 0) {
          this.messageService.add({ severity: "error", summary: translations['Common']['Error'], detail: translations['Equipment']['SelectRow'], key: this.toastKey });
          return;
        }
        this.equipmentService.unassignLoading = true;
        this.equipmentService.unassign(
          this.selectedEquipments).subscribe({
            next: _ => {
              this.equipmentService.unassignLoading = false;
              this.equipmentService.onUnassigned.emit();
              this.selectedEquipments = [];
              this.refreshEquipment(CRUDActions.Delete);
            },
            error: (error: HttpErrorResponse) => {
              this.equipmentService.unassignLoading = false;
            }
          });
      });
    });
  }

  onRefresh(): Subscription {
    return this.equipmentService.onAssigned.subscribe(()=>this.refreshEquipment(CRUDActions.Create));
  }

  loadEquipment(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || event.rows === undefined)
      return;

    let userId:number;

    userId = this.storageService.getUserData()?.id ?? 0;

    let filter:EquipmentFilter = {
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? "",
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1,
      PageSize: event.rows ?? 10,
      UserId: userId
    };
    if(event.filters !== undefined) {
      filter.ItemId = event.filters["itemId"]?.value ?? 0;
      filter.CategoryId = event.filters["categoryId"]?.value ?? 0;
    }

    this.equipments = [];
    this.equipmentService.getEquipmentsForCurrentUser(filter).subscribe({
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
