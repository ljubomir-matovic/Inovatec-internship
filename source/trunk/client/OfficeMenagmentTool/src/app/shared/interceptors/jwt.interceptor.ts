import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { StorageService } from '../helpers/storage.service';
import { Router } from '@angular/router';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private router: Router, private storageService: StorageService) {}

  intercept(httpRequest: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token: string | null = this.storageService.getToken();

    let jwtHttpRequest: HttpRequest<unknown> = httpRequest.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
    });

    return next.handle(jwtHttpRequest).pipe( catchError( (error: HttpErrorResponse) => this.handleUnauthorized(error) ));
  }

  private handleUnauthorized(error: HttpErrorResponse): Observable<any> {
    if(error.status === 401) {
      this.storageService.deleteToken();
      this.storageService.deleteUserData();
      let searchParams = new URLSearchParams();
      searchParams.set("redirectTo", this.router.routerState.snapshot.url);
      let queryParams = `?${searchParams.toString()}`
      this.router.navigateByUrl(`/login${queryParams}`);
    }

    if(error.status === 403) {
      this.router.navigate(["/"]);
    }

    return throwError(() => error);
  }

}
