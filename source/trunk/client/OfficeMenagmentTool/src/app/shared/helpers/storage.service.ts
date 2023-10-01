import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { User } from '../models/user.model';
import { ROLES, Role } from '../constants/role';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor() { }

  public storeToken(token: string): void {
    localStorage.setItem(environment.jwtKey, token);
  }

  public getToken(): string | null {
    return localStorage.getItem(environment.jwtKey);
  }

  public deleteToken(): void {
    localStorage.removeItem(environment.jwtKey);
  }

  public storeUserData(userData: User): void {
    localStorage.setItem(environment.userDataKey, JSON.stringify(userData));
  }

  public getUserData(): User | null {
    let stringUserData: string | null = localStorage.getItem(environment.userDataKey);
    if(stringUserData == null)
      return null;
    return JSON.parse(stringUserData);
  }

  public userAuthenticated(...roles: Role[]): boolean {
    let loggedUser: User | null = this.getUserData();
    if(loggedUser == null)
      return false;

    return roles.includes(this.getUserData()!.role);
  }

  checkIsCurrentUser(id: number): boolean {
    let user: User | null = this.getUserData();
    if(user === null || user.id !== id ) {
      return false;
    }
    return true;
  }

  public deleteUserData(): void {
    localStorage.removeItem(environment.userDataKey);
  }

  public getFullNameOfUser(): string {
    let user = this.getUserData();
    return `${user?.firstName ?? ""} ${user?.lastName ?? ""}`;
  }
}
