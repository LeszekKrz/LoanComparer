import { Component, OnDestroy, OnInit } from '@angular/core';
import { finalize, Observable, of, Subscription, switchMap } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { CreateInquiryComponent } from '../create-inquiry-page/create-inquiry.component';
import { Inquiry } from './models/inquiry';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  peopleUsingAppCount: number = 100; // we should fetch it from backend
  inquiries: Inquiry[] = []; // we should fetch it
  isProgressSpinnerVisible: boolean = false;
  subscriptions: Subscription[] = [];
  isUserAuthenticated!: boolean;

  constructor(private authenticationHttpService: AuthenticationService) {
    this.isUserAuthenticated = this.authenticationHttpService.isUserAuthenticated();
    this.subscriptions.push(
      this.authenticationHttpService.authenticationStateChanged.subscribe(isAuthenticated => {
        this.isUserAuthenticated = isAuthenticated;
    }));
  }

  ngOnInit(): void {
    this.inquiries = [{loanValue: 100, dateOfInquirySubmition: new Date('2022-12-07'), status: 'WAITING'},
    {loanValue: 1000, dateOfInquirySubmition: new Date('2022-11-02'), status: 'OFFERSCREATED'},
    {loanValue: 109.99, dateOfInquirySubmition: new Date('2022-12-01'), status: 'SUBMITTED'},
    {loanValue: 1000, dateOfInquirySubmition: new Date('2022-11-02'), status: 'ACCEPTED'},
    {loanValue: 109.99, dateOfInquirySubmition: new Date('2022-12-01'), status: 'REJECTED'}]; // we should fetch it here
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

}
