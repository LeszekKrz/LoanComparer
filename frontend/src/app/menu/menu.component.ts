import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthenticationService } from '../authentication/services/authentication.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit, OnDestroy {
  isUserAuthenticated!: boolean;
  isUserBankEmployee!: boolean;
  subscriptions: Subscription[] = [];

  constructor(private router: Router, private authenticationHttpService: AuthenticationService) {
    this.isUserAuthenticated = this.authenticationHttpService.isUserAuthenticated();
    this.isUserBankEmployee = this.authenticationHttpService.isUserBankEmployee();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.authenticationHttpService.authenticationStateChanged.subscribe(isAuthenticated => {
        this.isUserAuthenticated = isAuthenticated;
        this.isUserBankEmployee = this.authenticationHttpService.isUserBankEmployee();
    }));
  }

  BrandLogoOnClick(): void {
    this.router.navigate(['home']);
  }

  loginButtonOnClick(): void {
    this.router.navigate(['login']);
  }

  registerButtonOnClick(): void {
    this.router.navigate(['register']);
  }

  logoutButtonOnClick(): void {
    this.authenticationHttpService.logout();
  }

  adminPanelButtonOnClick(): void {
    this.router.navigate(['admin-panel']);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }
}
