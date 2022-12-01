import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig } from 'primeng/api';
import { AuthenticationService } from './authentication/services/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'loan-comparer';

  constructor(private primengConfig: PrimeNGConfig, private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
    this.primengConfig.ripple = true;
    if (this.authenticationService.isUserAuthenticated()) {
      this.authenticationService.sendAuthenticationStateChangedNotification(true);
    }
  }
}
