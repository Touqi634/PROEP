import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../app/services/auth.service';


@Injectable({
  providedIn: 'root'
})
export class ChildGuardGuard implements CanActivate {

  constructor(private auth: AuthService, private router: Router) {
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean {
      //check if the user is child
      if(this.auth.returnUserType() == "child")
      {
        return true;
      }
    this.router.navigate(['/login']);
    return false;
  }
  
}
