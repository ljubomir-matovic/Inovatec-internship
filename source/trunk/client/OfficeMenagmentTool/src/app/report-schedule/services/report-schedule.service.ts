import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { ReportSchedule } from 'src/app/shared/models/report-schedule';
import { ReportScheduleFilter } from 'src/app/shared/models/report-schedule-filter';
import { UpdateReportSchedule } from 'src/app/shared/models/update-report-schedule-model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReportScheduleService {
  private readonly apiUrl: string = `${environment.serverUrl}/api/ReportSchedule`;

  constructor(private http: HttpClient) { }

  getReportSchedules(reportScheduleFilter: ReportScheduleFilter): Observable<DataPage<ReportSchedule>> {
    return this.http.get<DataPage<ReportSchedule>>(this.apiUrl, { params: AppUtility.getParamsFromObject(reportScheduleFilter) });
  }

  getReportSchedule(id: number): Observable<ReportSchedule> {
    return this.http.get<ReportSchedule>(`${this.apiUrl}/${id}`);
  }

  addReportSchedule(office: UpdateReportSchedule): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(this.apiUrl, office);
  }

  updateReportSchedule(office: UpdateReportSchedule): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(this.apiUrl, office);
  }

  deleteReportSchedule(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
