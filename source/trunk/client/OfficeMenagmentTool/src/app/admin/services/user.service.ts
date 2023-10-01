import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { UserFilter } from 'src/app/shared/models/user-filter.model';
import { User } from 'src/app/shared/models/user.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl:string = `${environment.serverUrl}/api/User`;

  constructor(private http:HttpClient, private storageService: StorageService, private router: Router) {
  }

  getAllUsers(filter: UserFilter): Observable<DataPage<User>> {
    return this.http.get<DataPage<User>>(this.apiUrl, { params: AppUtility.getParamsFromObject(filter) });
  }

  getUserById(id: number): Observable<User> {
    let url = `${this.apiUrl.toString()}/${id}`;
    return this.http.get<User>(url);
  }

  createUser(user: User): Observable<any> {
    return this.http.post(this.apiUrl, user);
  }

  getCSVTemplate(): Observable<any> {
    return this.http.get(`${this.apiUrl}/csvTemplate`);
  }

  addUsersFromCSV(formData: FormData): Observable<ActionResultResponse<string>> {
    return this.http.post<ActionResultResponse<string>>(`${this.apiUrl}/fromCSV`, formData);
  }
  
  updateUser(user: User): Observable<any> {
    return this.http.put(this.apiUrl, user);
  }

  deleteUser(id: number): Observable<any> {
    let url=this.apiUrl+"/"+id;
    return this.http.delete(url);
  }

  getPersonalData(): Observable<any> {
    return this.http.get(`${this.apiUrl}/personal-data`);
  }

  changePersonalData(requestBody: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/personal-data`, requestBody);
  }

  changePassword(oldPassword: string, newPassword: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/change-password`, { oldPassword, newPassword});
  }

  logout(): void {
    this.storageService.deleteToken();
    this.storageService.deleteUserData();
    this.router.navigate([`/login`]);
  }
}
