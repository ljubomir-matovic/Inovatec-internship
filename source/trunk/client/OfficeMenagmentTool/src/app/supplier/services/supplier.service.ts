import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Supplier } from 'src/app/shared/models/supplier';
import { SupplierFilter } from 'src/app/shared/models/supplier-filter';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {
  
  private readonly apiUrl: string = `${environment.serverUrl}/api/Supplier`;

  constructor(private http: HttpClient) { }

  getSuppliers(supplierFilter: SupplierFilter): Observable<DataPage<Supplier>> {
    return this.http.get<DataPage<Supplier>>(this.apiUrl, { params: AppUtility.getParamsFromObject(supplierFilter) });
  }

  addSupplier(supplier: Supplier): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(this.apiUrl, supplier);
  }

  updateSupplier(supplier: Supplier): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(this.apiUrl, supplier);
  }

  deleteSupplier(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
