import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Office } from 'src/app/shared/models/office';
import { OfficeFilter } from 'src/app/shared/models/office-filter.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OfficeService {

  private readonly apiUrl: string = `${environment.serverUrl}/api/Office`;

  constructor(private http: HttpClient) { }

  getOffices(officeFilter: OfficeFilter): Observable<ActionResultResponse<DataPage<Office>>> {
    return this.http.get<ActionResultResponse<DataPage<Office>>>(this.apiUrl, { params: AppUtility.getParamsFromObject(officeFilter) });
  }

  addOffice(office: Office): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(this.apiUrl, office);
  }

  updateOffice(office: Office): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(this.apiUrl, office);
  }

  deleteOffice(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
