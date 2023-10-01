import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ProblemReport } from 'src/app/shared/models/problem-report';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Observable } from 'rxjs';
import { OrderFilter } from 'src/app/shared/models/order-filter';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Order } from 'src/app/shared/models/order';
import { UpdateReport } from 'src/app/shared/models/update-problem';
import { AppUtility } from 'src/app/shared/functions/utility';
import { SnackOrder } from 'src/app/shared/models/snack-order';
import { UpdateOrder } from 'src/app/shared/models/update-order';
import { EquipmentOrder } from 'src/app/shared/models/equipment-order';
import { SnackOrderUpdate } from 'src/app/shared/models/snack-order-update';

@Injectable({
  providedIn: 'root'
})
export class OrderRequestService {
  private readonly apiUrl: string = `${environment.serverUrl}/api/OrderRequest`;

  constructor(private http: HttpClient) { }

  getReportedProblems(orderFilter: OrderFilter): Observable<ActionResultResponse<DataPage<Order>>> {
    return this.http.get<ActionResultResponse<DataPage<Order>>>(`${this.apiUrl}/report`, { params: AppUtility.getParamsFromObject(orderFilter) });
  }

  reportProblem(problemReport: ProblemReport): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(`${this.apiUrl}/report`, problemReport);
  }

  deleteReport(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/report/${id}`);
  }

  updateReport(reportForm: UpdateReport): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(`${this.apiUrl}/report`, reportForm);
  }

  getSnackOrders(orderFilter: OrderFilter): Observable<ActionResultResponse<DataPage<Order>>>  {
    return this.http.get<ActionResultResponse<DataPage<Order>>>(`${this.apiUrl}`, { params: AppUtility.getParamsFromObject(orderFilter) });
  }

  getSnackOrdersGroupedByItem(orderFilter: OrderFilter): Observable<ActionResultResponse<DataPage<Order>>>  {
    return this.http.get<ActionResultResponse<DataPage<Order>>>(`${this.apiUrl}/groupByItem`, { params: AppUtility.getParamsFromObject(orderFilter) });
  }

  addSnackOrder(snackOrders: SnackOrder): Observable<ActionResultResponse<Order>>  {
    return this.http.post<ActionResultResponse<Order>>(`${this.apiUrl}`, snackOrders);
  }

  updateSnackOrder(orderUpdateData: SnackOrderUpdate): Observable<ActionResultResponse<string>>  {
    return this.http.patch<ActionResultResponse<string>>(`${this.apiUrl}`, orderUpdateData);
  }

  deleteSnackOrder(id: number): Observable<ActionResultResponse<string>>  {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }

  updateOrder(orderForm: UpdateOrder): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(`${this.apiUrl}/equipment`, orderForm);
  }

  getEquipmentOrder(orderFilter: OrderFilter) {
    return this.http.get<ActionResultResponse<DataPage<Order>>>(`${this.apiUrl}/equipment`, { params: AppUtility.getParamsFromObject(orderFilter) });
  }

  orderEquipment(orderData: EquipmentOrder): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(`${this.apiUrl}/equipment`, orderData);
  }

  deleteOrder(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
