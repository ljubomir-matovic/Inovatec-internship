import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AppUtility } from "src/app/shared/functions/utility";
import { ActionResultResponse } from "src/app/shared/models/action-result-response.model";
import { DataPage } from "src/app/shared/models/data-page.model";
import { ProblemReport } from "src/app/shared/models/problem-report";
import { Report } from "src/app/shared/models/report";
import { ReportFilter } from "src/app/shared/models/report-filter";
import { UpdateReport } from "src/app/shared/models/update-problem";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private readonly apiUrl: string = `${environment.serverUrl}/api/Report`;

  constructor(private http: HttpClient) { }

  getReports(reportsFilter: ReportFilter): Observable<ActionResultResponse<DataPage<Report>>> {
    return this.http.get<ActionResultResponse<DataPage<Report>>>(`${this.apiUrl}`, { params: AppUtility.getParamsFromObject(reportsFilter) });
  }

  reportProblem(problemReport: ProblemReport): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(`${this.apiUrl}`, problemReport);
  }

  updateReport(updateReport: UpdateReport): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(`${this.apiUrl}`, updateReport);
  }

  deleteReport(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
