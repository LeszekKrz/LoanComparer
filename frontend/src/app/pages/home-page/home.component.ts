import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { finalize, Observable, of, Subscription, switchMap } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { applicationDescription } from './home-page-constants';
import { Inquiry } from './models/inquiry';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  registeredUsersCount: number = 100; // we should fetch it from backend
  inquiries: Inquiry[] = []; // we should fetch it
  isProgressSpinnerVisible: boolean = false;
  subscriptions: Subscription[] = [];
  isUserAuthenticated!: boolean;
  applicationDescription: String = applicationDescription;

  constructor(private authenticationHttpService: AuthenticationService, private router: Router) {
    this.isUserAuthenticated = this.authenticationHttpService.isUserAuthenticated();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.authenticationHttpService.authenticationStateChanged.subscribe(isAuthenticated => {
        this.isUserAuthenticated = isAuthenticated;
    }));
    this.inquiries = [{id: '1', loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-12-07'), status: 'WAITINGFORACCEPTANCE'},
    {id: '2', loanValue: 1000, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-11-02'), status: 'OFFERSCREATED'},
    {id: '3', loanValue: 109.99, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-12-01'), status: 'SUBMITTED'},
    {id: '4', loanValue: 1000, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-11-02'), status: 'ACCEPTED'},
    {id: '5', loanValue: 109.99, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-12-01'), status: 'REJECTED'}]; // we should fetch it here
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  createNewInquiryOnClickHandler(): void {
    this.router.navigate(['create-inquiry']);
  }

  onRowClickHandler(inquiry: Inquiry) {
    if (inquiry.status == 'SUBMITTED' || inquiry.status == 'OFFERSCREATED') {
      this.router.navigate([`choose-offer/${inquiry.id}`])
    }
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }
}
