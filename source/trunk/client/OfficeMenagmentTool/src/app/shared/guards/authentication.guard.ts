import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { StorageService } from '../helpers/storage.service';
import { Role } from '../constants/role';
import { User } from '../models/user.model';
import { AppRoutes } from '../constants/routes';

export const AnonymousGuard: CanActivateFn = (ars: ActivatedRouteSnapshot, rss: RouterStateSnapshot) => {
  const storageService = inject(StorageService);
  const router = inject(Router);

  let token = storageService.getToken();
  let userData: User | null = storageService.getUserData();

  if(token != undefined && userData != null){
    router.navigate(['/']);
    return false;
  }

  return true;
};

export function AuthorizedGuard(...permittedRoles: Role[]): CanActivateFn {
  return (ars: ActivatedRouteSnapshot, rss: RouterStateSnapshot) => {
    const storageService = inject(StorageService);
    const router = inject(Router);
    let token = storageService.getToken();
    let userData = storageService.getUserData();

    if(token == undefined || userData == null){
      let searchParams = new URLSearchParams();
      searchParams.set("redirectTo", rss.url);
      let queryParams = `?${searchParams.toString()}`;
      router.navigateByUrl(`/login${queryParams}`);
      return false;
    }

    if(permittedRoles.length == 0)
      return true;
    
    let canActivate: boolean = false;
    permittedRoles.forEach(role => {
      if(userData?.role === role)
        canActivate = true;
    });

    if(!canActivate || (rss.url === "/")) {
      redirectByRole(router, userData?.role);
      return false;
    }
    return true;
  }

};

function redirectByRole(router: Router, role: Role) {
  switch(role) {
    case Role.Admin:
      router.navigate([AppRoutes.admin.defaultRoute]);
      break;
    case Role.HR:
      router.navigate([AppRoutes.hr.defaultRoute]);
      break;
    default:
      router.navigate([AppRoutes.user.defaultRoute]);
  }
}
