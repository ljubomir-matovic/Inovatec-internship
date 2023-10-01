import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService, FilterMatchMode, LazyLoadEvent, MessageService, SelectItem } from 'primeng/api';
import { User } from 'src/app/shared/models/user.model';
import { UserService } from '../../services/user.service';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CreateUserComponent } from '../create-user/create-user.component';
import { ROLES } from 'src/app/shared/constants/role';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { UserFilter } from 'src/app/shared/models/user-filter.model';
import { Table } from 'primeng/table';
import { AuthService } from 'src/app/shared/services/auth.service';
import { TranslateService } from '@ngx-translate/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AddCSVComponent } from '../add-csv/add-csv.component';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit,OnDestroy{
  lastLoadEvent!: LazyLoadEvent;
  users: User[] = [];
  totalRecords: number = 1;
  loading: boolean=true;
  ref!: DynamicDialogRef;
  dialogStatus!:EventEmitter<any>;
  first: number = 0;
  rows: number = 10;
  position: string = "";

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  subscriptions: EventEmitter<any>[] = [];
  onDialogClose: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();
  roles=ROLES;

  onFilesUploaded: EventEmitter<ActionResult<any>> = new EventEmitter<ActionResult<any>>();

  get toastKey():string {
    return "toast";
  }

  constructor(
    private userService: UserService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private authService: AuthService,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.initSubscriptions();
    this.ref = new DynamicDialogRef();
    this.dialogStatus = new EventEmitter<any>();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  getPageNumber(first: number, rows: number):number {
    return ( first / rows ) + 1;
  }

  initSubscriptions(): void {
    this.subscriptions.push(this.onDialogClose);
    this.subscriptions.push(this.onFilesUploaded);

    this.onDialogClose.subscribe({
      next: (status: ActionResult<any>) => {
        this.dialogClosed(status);
      }
    });

    this.onFilesUploaded.subscribe({
      next: (status: ActionResult<any>) => {
        this.filesUploaded(status);
      }
    })
  }

  dialogClosed(status: ActionResult<any>): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      if(status.success) {
        this.refreshUsers(status.data.action);
        this.ref.close();
        this.messageService.add({ severity: "success", summary: translations['Common']["Success"], detail: translations['UserAdministration'][status.data.message], key: this.toastKey });
      } 
      else {
        this.messageService.add({ severity: "error", summary: translations["Common"]["Error"], detail: translations['UserAdministration'][status.data.message], key: this.toastKey });
      }
    });
  }

  loadUsers(event: LazyLoadEvent): void {
    this.lastLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || event.rows === undefined)
      return;

    let filter: UserFilter = {
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1
    };

    if(event.filters !== undefined) {
      filter.FirstName = event.filters["firstName"]?.value ?? null;
      filter.LastName = event.filters["lastName"]?.value ?? null;
      filter.Email = event.filters["email"]?.value ?? null;
      filter.Roles = event.filters["roles"]?.value?.map((x:any) => x.id) ?? null;
    }
    this.users = [];

    this.userService.getAllUsers(filter).subscribe({
      next:(value:DataPage<User>) => {
        this.totalRecords = value.totalRecords;
        this.users = value.data;
        this.loading = false;
      },
      error:(err:any) => {
        this.loading = false;
        this.users = [];
        this.first = 0;
      }
    });
  }

  refreshUsers(action:CRUDActions): void {
    switch(action){
      case CRUDActions.Create:
      case CRUDActions.Update:
        this.loadUsers({ ...this.lastLoadEvent, first:this.first, rows:this.rows});
        break;
      case CRUDActions.Delete:
        let first = this.first;
        let pageNumber = this.getPageNumber(first, this.rows);

        if((pageNumber - 1) * this.rows == this.totalRecords - 1 && pageNumber > 1){
          this.loadUsers({ ...this.lastLoadEvent, first: first-this.rows, rows: this.rows });
        } else {
          this.loadUsers({ ...this.lastLoadEvent, first: this.first, rows: this.rows });
        }

        break;
    }
  }

  showAddUser(): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.ref=this.dialogService.open(CreateUserComponent,{
        header:translations['UserAdministration']['AddUserLabel'],
        maximizable: true,
        data:{
          formSubmitted:this.onDialogClose
        }
      });
    });
  }

  showUpdateUser(user:User): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.ref=this.dialogService.open(CreateUserComponent,{
        header: translations['UserAdministration']['UpdateUserLabel'],
        maximizable: true,
        data:{
          user,
          formSubmitted:this.onDialogClose
        }
      });
    });
  }

  showDeleteConfirm(id:number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.confirmationService.confirm({
        message: translations['UserAdministration']['DeleteUserConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.userService.deleteUser(id).subscribe({
            next:(msg) => {
              this.refreshUsers(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['UserAdministration'][msg.actionData], key: this.toastKey });
            }
          })
        },
        reject: (type:any) => {
            this.confirmationService.close();
        }
      });
    });
  }

  clearFilters(table:Table):void {
    table.clear();
  }

  applyFilter(event:any,table:Table,field:string):void {
    table.filter(event.target.value,field,this.matchMode);
  }

  showCreateResetPasswordTokenConfirm(email:string) {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations =>{
      this.confirmationService.confirm({
        message: translations['UserAdministration']['ResetPasswordConfirmationDescription'],
        header: translations['UserAdministration']['ResetPasswordConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.authService.forgotPassword(email).subscribe({
            next:() => {
              this.refreshUsers(CRUDActions.Delete);
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['UserAdministration']['TokenCreatedSuccess'], key: this.toastKey });
            },
            error:(err:HttpErrorResponse) => {
              let error = err.error[0];
              this.messageService.add({ severity: 'error', summary: translations['Common']['Error'], detail: translations['UserAdministration'][error], key: this.toastKey })
            }
          })
        },
        reject: (type:any) => {
            this.confirmationService.close();
        }
      });
    });
  }

  showAddCSV(): void {
    this.ref = this.dialogService.open(AddCSVComponent, {
      header: this.translateService.instant('UserAdministration.AddUsersFromCSVLabel'),
      maximizable: false,
      data:{
        csvSubmitted: this.onDialogClose,
        filesUploaded: this.onFilesUploaded
      }
    });
  }

  filesUploaded(status: ActionResult<any>): void {
    if(status.success) {
      this.ref.close();
      this.refreshUsers(CRUDActions.Create);
      this.messageService.add({ severity: "success", summary: this.translateService.instant('Common.Success'), detail: this.translateService.instant(`UserAdministration.${status.data.message}`), key: this.toastKey });
    } 
    else {
      this.ref.close();
      if(status.data.message) {
        this.messageService.add({ severity: "error", summary: this.translateService.instant('Common.Error'), detail: this.translateService.instant(`UserAdministration.${status.data.message}`), key: this.toastKey });
      }
      else {
        this.messageService.add({ severity: "error", summary: this.translateService.instant('Common.Error'), key: this.toastKey });
      }
    }
  };
}
