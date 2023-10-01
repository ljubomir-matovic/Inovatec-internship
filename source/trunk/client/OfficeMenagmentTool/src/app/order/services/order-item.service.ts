import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { CategoryTypes } from 'src/app/shared/constants/category-types';
import { CategoriesService } from 'src/app/products/services/categories.service';
import { ItemService } from 'src/app/products/services/item.service';
import { EquipmentFilter } from 'src/app/shared/models/equipment-filter.model';

@Injectable({
  providedIn: 'root'
})
export class OrderItemService {

  private readonly apiUrl: string = `${environment.serverUrl}/api/OrderItem`;

  constructor(private http: HttpClient, private categoryService: CategoriesService, private itemService: ItemService) { }

  getOrderItemsPage(id: number, filter: EquipmentFilter): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`, { params: AppUtility.getParamsFromObject(filter) });
  }

  getCategories(): Observable<any> {
    return this.categoryService.getCategories({
      PageNumber: 1,
      PageSize: 1,
      Types: [CategoryTypes.Snacks],
      SortOrder: 1
    });
  }

  getItems(category?:any): Observable<any> {
    let categoryArray:any[] = [];

    if(category != null) {
      categoryArray.push(category);
    }

    return this.itemService.getItems({
      PageNumber: 1,
      PageSize: 100,
      CategoryType: CategoryTypes.Snacks,
      Categories: categoryArray,
      SortOrder: 1
    });
  }

  createOrderItem(body: any): Observable<any> {
    return this.http.post(this.apiUrl, body);
  }

  changeAmount(orderId:number, id: number, amount: number): Observable<any> {
    return this.http.put(this.apiUrl,{ id, amount, orderId, itemId: 0});
  }

  deleteSelectedOrderItems(orderItems: any[]): Observable<any> {
    return this.http.delete(this.apiUrl,{ body:orderItems.map(ord => ord.id) });
  }

  deleteOrderItem(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
