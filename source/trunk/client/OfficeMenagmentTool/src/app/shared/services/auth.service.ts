import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { ActionResultResponse } from "../models/action-result-response.model";
import { LoginResponse } from "../models/login-response";
import { StorageService } from "../helpers/storage.service";

@Injectable({
  providedIn:"root"
})
export class AuthService{
  private readonly apiUrl:string = `${environment.serverUrl}/api/Auth`;

  constructor(private http:HttpClient){}

  resetPassword(token:string,password:string): Observable<any> {
    return this.http.put(`${this.apiUrl}/reset-password`,{ token, password });
  }

  forgotPassword(email:string): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`,{ email });
  }

  loginUser(loginData: any): Observable<ActionResultResponse<LoginResponse>> {
    return this.http.post<any>(`${this.apiUrl}/login`, loginData);
  }
}
