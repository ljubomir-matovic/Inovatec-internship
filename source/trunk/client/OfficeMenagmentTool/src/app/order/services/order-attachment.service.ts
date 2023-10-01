import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderAttachmentService {
  private apiUrl: string = `${environment.serverUrl}/api/OrderAttachments`;

  constructor(private http: HttpClient) { }

  uploadFiles(formData: FormData): Observable<any> {
    return this.http.post(this.apiUrl, formData);
  }

  getAttachmentsForOrder(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/order/${id}`);
  }

  getAttachmentById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  deleteFile(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
