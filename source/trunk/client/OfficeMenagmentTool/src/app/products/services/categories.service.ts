import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Category } from 'src/app/shared/models/category';
import { CategoryFilter } from 'src/app/shared/models/category-filter.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  private readonly apiUrl: string = `${environment.serverUrl}/api/Category`;

  constructor(private http: HttpClient) { }

  getCategoriesByType(type: number): Observable<ActionResultResponse<Category[]>> {
    let params: HttpParams = new HttpParams().set("Types", type);
    return this.http.get<ActionResultResponse<Category[]>>(this.apiUrl, { params: params });
  }

  getCategories(categoryFilter: CategoryFilter): Observable<ActionResultResponse<Array<Category>>> {
    return this.http.get<ActionResultResponse<Array<Category>>>(`${this.apiUrl}`, { params: AppUtility.getParamsFromObject(categoryFilter) });
  }

  getFilteredCategories(categoryFilter: CategoryFilter): Observable<ActionResultResponse<DataPage<Category>>> {
    return this.http.get<ActionResultResponse<DataPage<Category>>>(`${this.apiUrl}/page`, { params: AppUtility.getParamsFromObject(categoryFilter) });
  }

  createCategory(category: Category): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(this.apiUrl, category);
  }

  updateCategory(category: Category): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(this.apiUrl, category);
  }

  deleteCategory(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
