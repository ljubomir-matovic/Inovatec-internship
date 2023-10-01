import { ChangeDetectorRef, Component } from '@angular/core';
import { FilterMatchMode, LazyLoadEvent } from 'primeng/api';
import { Log } from 'src/app/shared/models/log';
import { LogsService } from '../../services/logs.service';
import { LogsFilter } from 'src/app/shared/models/logs-filter';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Table } from 'primeng/table';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';

@Component({
  selector: 'app-logs-administration',
  templateUrl: './logs-administration.component.html',
  styleUrls: ['./logs-administration.component.scss']
})
export class LogsAdministrationComponent {
  lazyLoadEvent!: LazyLoadEvent;
  logs: Log[] = [];
  totalRecords: number = 1;
  loading: boolean=true;
  first: number = 0;
  rows: number = 10;
  position: string = "";

  selectedLog?: Log | null;

  get matchMode(): string {
    return FilterMatchMode.CONTAINS;
  }

  get toastKey(): string {
    return "toast";
  }

  constructor(
    private logsService: LogsService,
    private changeDetectorRef: ChangeDetectorRef
  ) { }

  ngOnInit(): void {

  }

  getPageNumber(first:number,rows:number):number {
    return ( first / rows ) + 1;
  }

  lazyLoadLogs(event: LazyLoadEvent): void {
    this.lazyLoadEvent = event;
    this.first = event?.first ?? 0;
    this.loading = true;

    if(event.first === undefined || event.rows === undefined)
      return;

    let filter: LogsFilter = {
      PageSize: 10,
      PageNumber: this.getPageNumber(event.first, event.rows),
      SortField: event.sortField ?? null,
      SortOrder: (event.sortOrder !== undefined && event.sortOrder < 0) ? -1 : 1,
    };

    if(event.filters !== undefined) {
      filter.Message = event.filters["message"]?.value ?? null;
      filter.Exception = event.filters["exception"]?.value ?? null;
      filter.User = event.filters["user"]?.value ?? null;
    }

    this.logs = [];

    this.logsService.getLogs(filter).subscribe({
      next:(result: ActionResultResponse<DataPage<Log>>) => {
        if(result.actionSuccess == false || result.actionData == null) {
          this.loading = false;
          this.logs = [];
          this.first = 0;
          return;
        }

        this.totalRecords = result.actionData.totalRecords;
        this.logs = result.actionData.data;
        this.loading = false;
        this.changeDetectorRef.detectChanges();

        if(this.logs[0] != null && this.logs[0] != undefined) {
          this.openLog(this.logs[0]);
        }
      },
      error:(err:any) => {
        this.loading = false;
        this.logs = [];
        this.first = 0;
      }
    });
  }

  clearFilters(table: Table):void {
    table.clear();
  }

  applyFilter(event: any, table: Table, field: string): void {
    table.filter(event.target.value,field,this.matchMode);
  }

  copyLog(log: Log): void {
    navigator.clipboard.writeText(log.exception);
  }

  openLog(log: Log): void {
    this.selectedLog = log;
  }
}
