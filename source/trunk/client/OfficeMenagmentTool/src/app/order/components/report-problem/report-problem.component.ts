import { Component } from '@angular/core';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Router } from '@angular/router';
import { AppRoutes } from 'src/app/shared/constants/routes';
import { FilterMatchMode, LazyLoadEvent, MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { REPORT_CATEGORIES, ReportCategories } from 'src/app/shared/constants/report-categories';
import { Equipment } from 'src/app/shared/models/equipment.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { EquipmentFilter } from 'src/app/shared/models/equipment-filter.model';
import { EquipmentService } from 'src/app/products/services/equipment.service';
import { ProblemReport } from 'src/app/shared/models/problem-report';
import { Category } from 'src/app/shared/models/category';
import { Table } from 'primeng/table';
import { Item } from 'src/app/shared/models/item';
import { ReportService } from '../../services/report.service';
import { Office } from 'src/app/shared/models/office';
import { OfficeService } from 'src/app/office/services/office.service';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { User } from 'src/app/shared/models/user.model';
import { OrderRequestService } from '../../services/order-request.service';

@Component({
  selector: 'app-report-problem',
  templateUrl: './report-problem.component.html',
  styleUrls: ['./report-problem.component.scss']
})
export class ReportProblemComponent {
  reportCategory: any = ReportCategories;
  reportCategories: any = REPORT_CATEGORIES;

  descriptionText: string = "";

  categories!: Category[];
  selectedReportCategory: any = this.reportCategories[ReportCategories.Equipment];

  offices!: Office[];
  selectedOffice!: any;

  items!: Item[];

  submittingOrder: boolean = false;
  equipmentList!: Equipment[];
  lazyLoadEvent!: LazyLoadEvent;
  totalRecords!: number;
  pageSize: number = 5;
  first: number = 0;
  loadingEquipment!: boolean;
  selectedEquipment!: Equipment | null;

  sending: boolean = false;

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  get toastKey():string {
    return "toast";
  }

  get submitReportIsValid(): boolean {
    if(this.descriptionText.trim() === "")
      return false;

    if(this.selectedOffice.id === null) {
      return false;
    }

    if(this.selectedReportCategory.id === ReportCategories.Equipment && this.selectedEquipment === null)
      return false;

    return true;
  }

  constructor(
    private storageService: StorageService,
    private messageService: MessageService,
    private equipmentService: EquipmentService,
    private translateService: TranslateService,
    private reportService: ReportService,
    private officeService: OfficeService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.setCategories();
    this.setOffices();
    this.setItems();
  }

  getPageNumber(first: number, pageSize: number): number {
    return ( first / pageSize ) + 1;
  }

  setOffices(): void {
    let officesFilter: OfficeFilter = {
      PageNumber: 0,
      SortField: "name",
      SortOrder: 1,
      PageSize: 0
    };

    this.officeService.getOffices(officesFilter).subscribe(
      {
        next: (result: ActionResultResponse<DataPage<Office>>) => {
          if(!result.actionSuccess || result.actionData == null) {
            this.offices = [];
            return;
          }

          this.offices = result.actionData.data;

          let currentUser: User | null = this.storageService.getUserData();

          if(currentUser != null) {
            this.selectedOffice = this.offices.find(office => office.id === currentUser?.officeId);
          }
        },
        error: (error: any) => {
          this.offices = [];
        }
      }
    )
  }

  setCategories(): void {
    this.equipmentService.getCategories().subscribe(result => {
      if(!result.actionSuccess) {
        return;
      }
      this.categories = result.actionData.data;
      this.selectedEquipment = null;
    })
  }

  setItems(category?: any): void {
    this.equipmentService.getItems(category).subscribe(result => {
      if(!result.actionSuccess) {
        return;
      }
      this.items = result.actionData.data;
      this.selectedEquipment = null;
    });
  }

  lazyLoadEquipment(event: LazyLoadEvent): void {
    this.lazyLoadEvent = event;
    this.first = event?.first ?? 0;

    if(event.first === undefined || event.rows === undefined)
      return;

    let filter: EquipmentFilter = {
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? "",
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1,
      PageSize: event.rows ?? 10
    };
    if(event.filters !== undefined) {
      filter.ItemId = event.filters["itemId"]?.value ?? 0;
      filter.CategoryId = event.filters["categoryId"]?.value ?? 0;
    }

    this.loadingEquipment = true;
    this.equipmentList = [];

    this.equipmentService.getEquipmentsForCurrentUser(filter).subscribe({
      next:(result: DataPage<Equipment>) => {
        this.totalRecords = result.totalRecords;
        this.equipmentList = result.data;
        this.loadingEquipment = false;
      },
      error:(err: any) => {
        this.loadingEquipment = false;
        this.first = 0;
      }
    });
  }

  filterEquipment(event: any, table: Table, field: string): void {
    table.filter(event.target.value, field, this.matchMode);
  }

  clearFilters(table: Table): void {
    table.clear();
  }

  submitReport(): void {
    if(!this.submitReportIsValid)
      return;

    let reportData: ProblemReport = {
      description: this.descriptionText,
      category: this.selectedReportCategory.id,
      officeId: this.selectedOffice.id
    }

    reportData.equipmentId = (this.selectedReportCategory.id === ReportCategories.Equipment) ? this.selectedEquipment!.id : null;

    this.reportService.reportProblem(reportData).subscribe({
      next: (actionResultResponse: ActionResultResponse<string>) => {
        if(actionResultResponse == null || !actionResultResponse.actionSuccess) {
          this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
            this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['ReportsAdministration'][actionResultResponse.errors[0]], key: this.toastKey });
          });
        }
        this.sending = false;

        this.router.navigate([AppRoutes.shared.myProblemsRoute]);
      },
      error: (error: any) => {
        this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
          this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['Common'][error.message], key: this.toastKey });
        });
      }
    });
  }
}
