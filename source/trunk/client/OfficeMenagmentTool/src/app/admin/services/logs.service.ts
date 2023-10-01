import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Log } from 'src/app/shared/models/log';
import { LogsFilter } from 'src/app/shared/models/logs-filter';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LogsService {
  private readonly apiUrl:string = `${environment.serverUrl}/api/Log`;

  constructor(private http: HttpClient) { }

  getLogs(filter: LogsFilter): Observable<ActionResultResponse<DataPage<Log>>> {
    return this.http.get<ActionResultResponse<DataPage<Log>>>(this.apiUrl, { params: AppUtility.getParamsFromObject(filter) });
  }
}
