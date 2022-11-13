import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { MessageService } from 'primeng/api';
import { insuficcientAccessMessage } from 'src/app/core/messages';
import { AuthenticationService } from '../services/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class BankEmployeeGuard implements CanActivate {
  constructor(private authenticationService: AuthenticationService, private messageService: MessageService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const isUserBankEmployee = this.authenticationService.isUserBankEmployee();
    if (!isUserBankEmployee) {
      this.messageService.add({severity: 'error', summary: 'Error', detail: insuficcientAccessMessage}); // TODO: should redirect to the last page
    }
    return isUserBankEmployee;
  }
}
