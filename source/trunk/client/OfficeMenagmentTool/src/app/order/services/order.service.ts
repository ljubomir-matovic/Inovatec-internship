import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Observable } from 'rxjs';
import { OrderFilter } from 'src/app/shared/models/order-filter';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { AppUtility } from 'src/app/shared/functions/utility';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private readonly apiUrl: string = `${environment.serverUrl}/api/Order`;

  constructor(private http: HttpClient) { }

  acceptOrders(): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(`${this.apiUrl}`,null);
  }

  getOrderById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  getOrdersPage(orderFilter: OrderFilter): Observable<ActionResultResponse<DataPage<any>>> {
    return this.http.get<ActionResultResponse<DataPage<any>>>(`${this.apiUrl}`, { params: AppUtility.getParamsFromObject(orderFilter) });
  }

  changeOrderState(id: number, state: number): Observable<any> {
    return this.http.put(this.apiUrl,{ id, state });
  }
}
