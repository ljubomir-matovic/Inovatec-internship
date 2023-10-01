import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { Item } from 'src/app/shared/models/item';
import { ItemFilter } from 'src/app/shared/models/item-filter.model';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class ItemService {
  private readonly apiUrl: string = `${environment.serverUrl}/api/Item`;

  constructor(private http: HttpClient) { }

  getItems(itemFilter: ItemFilter): Observable<ActionResultResponse<Array<Item>>> {
    return this.http.get<ActionResultResponse<Array<Item>>>(`${this.apiUrl}`, { params: AppUtility.getParamsFromObject(itemFilter) });
  }

  getItemsPage(itemFilter: ItemFilter): Observable<ActionResultResponse<DataPage<Item>>> {
    return this.http.get<ActionResultResponse<DataPage<Item>>>(`${this.apiUrl}/page`, { params: AppUtility.getParamsFromObject(itemFilter) });
  }

  createItem(item: Item): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(this.apiUrl, item);
  }

  updateItem(item: Item): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(this.apiUrl, item);
  }

  deleteItem(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
