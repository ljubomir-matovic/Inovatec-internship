import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { NotificationFilterRequest } from '../models/notification-filter-request.model';
import { NotificationViewModel } from '../models/notification.model';
import { AppUtility } from '../functions/utility';
import { DataPage } from '../models/data-page.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private readonly apiUrl: string = `${environment.serverUrl}/api/Notification`;

  menuVisible: boolean = false;

  constructor(private http: HttpClient) {
  }

  getNotifications(filterRequest: NotificationFilterRequest): Observable<DataPage<NotificationViewModel>> {
    return this.http.get<DataPage<NotificationViewModel>>(this.apiUrl, { params: AppUtility.getParamsFromObject(filterRequest) } );
  }

  unreadNotificationNumber(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/unreadNumber`);
  }

  markAsRead(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/markAsRead/${id}`, null);
  }
}
