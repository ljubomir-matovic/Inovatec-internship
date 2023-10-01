import { HttpClient, HttpParams } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CategoryTypes } from 'src/app/shared/constants/category-types';
import { EquipmentFilter } from 'src/app/shared/models/equipment-filter.model';
import { environment } from 'src/environments/environment';
import { CategoriesService } from './categories.service';
import { ItemService } from './item.service';

@Injectable({
  providedIn: 'root'
})
export class EquipmentService {
  apiUrl: string = `${environment.serverUrl}/api/Equipment`;
  onRefresh: EventEmitter<any> = new EventEmitter<any>();
  onRefreshUnassigned: EventEmitter<any> = new EventEmitter<any>();
  onRefreshAssigned: EventEmitter<any> = new EventEmitter<any>();
  onAssign: EventEmitter<any> = new EventEmitter<any>();
  onUnassign: EventEmitter<any> = new EventEmitter<any>();
  onAssigned: EventEmitter<any> = new EventEmitter<any>();
  onUnassigned: EventEmitter<any> = new EventEmitter<any>();

  assignLoading: boolean = false;
  unassignLoading: boolean = false;

  selectedUserId:number | null = null;

  constructor(private http:HttpClient, private categoryService:CategoriesService, private itemService:ItemService) { }

  getEquipments(filters:EquipmentFilter): Observable<any> {
    let params = new HttpParams({ fromObject: (filters as any) });
    return this.http.get(this.apiUrl,{ params });
  }

  getEquipmentsForCurrentUser(filters:EquipmentFilter): Observable<any> {
    let params = new HttpParams({ fromObject: (filters as any) });
    return this.http.get(`${this.apiUrl}/user`,{ params });
  }

  getCategories(): Observable<any> {
    return this.categoryService.getFilteredCategories({
      PageNumber: 1,
      PageSize: 50,
      Types: [CategoryTypes.Equipment],
      SortOrder: 1
    });
  }

  getItems(category?:any): Observable<any> {
    let categoryArray:any[] = [];

    if(category != null) {
      categoryArray.push(category);
    }

    return this.itemService.getItemsPage({
      PageNumber: 1,
      PageSize: 100,
      CategoryType: CategoryTypes.Equipment,
      Categories: categoryArray,
      SortOrder: 1
    });
  }

  createEquipment(body:any): Observable<any> {
    return this.http.post(`${this.apiUrl}`,{...body, userId:null});
  }

  assign(equipments:any[],userId:number): Observable<any> {
    return this.http.put(this.apiUrl,{equipments:equipments.map(eq => ({id: eq.id, itemId: eq.itemId})), userId});
  }

  unassign(equipments:any[]): Observable<any> {
    return this.http.put(this.apiUrl,{equipments:equipments.map(eq => ({id: eq.id, itemId: eq.itemId})), userId:null});
  }

  deleteSelectedEquipments(equipments:any[]): Observable<any> {
    return this.http.delete(this.apiUrl,{ body:equipments.map(eq => eq.id) });
  }

  deleteEquipment(id:number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
