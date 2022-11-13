import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { Subscription } from 'rxjs';
import { AuthenticationService } from '../authentication/services/authentication.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnDestroy {
  isUserAuthenticated!: boolean;
  subscriptions: Subscription[] = [];

  constructor(private router: Router, private authenticationHttpService: AuthenticationService) {
    this.subscriptions.push(
      this.authenticationHttpService.authenticationStateChanged.subscribe(isAuthenticated => {
        this.isUserAuthenticated = isAuthenticated;
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

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }
}
