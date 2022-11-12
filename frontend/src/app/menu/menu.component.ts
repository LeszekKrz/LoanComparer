import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { Subscription } from 'rxjs';
import { AuthenticationHttpService } from '../authentication/services/authentication.http.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit, OnDestroy {
  isUserAuthenticated!: boolean;
  subscriptions: Subscription[] = [];

  loginItem: MenuItem = {
    label: 'Login',
    routerLink: ['login'],
    visible: !this.isUserAuthenticated
  };

  registerItem: MenuItem = {
    label: 'Register',
    routerLink: ['register'],
    visible: !this.isUserAuthenticated
  };

  logoutItem: MenuItem = {
    label: 'Logout',
    command: () => console.log('logout'),
    visible: this.isUserAuthenticated
  }

  menuItems: MenuItem[] = [
    this.loginItem,
    this.registerItem,
    this.logoutItem
  ]

  constructor(private router: Router, private authenticationHttpService: AuthenticationHttpService) { }

  BrandLogoOnClick(): void {
    console.log('route to home page'); // route to home page
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

  ngOnInit(): void {
    this.subscriptions.push(
      this.authenticationHttpService.authenticationStateChanged.subscribe(isAuthenticated => {
        this.isUserAuthenticated = isAuthenticated;
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }
}
