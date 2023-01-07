import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { applicationDescription } from './home-page-constants';
import { InquiryDTO } from './models/inquiry-dto';
import { HomeHttpServiceService } from './services/home.http.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  registeredUsersCount: number = 100; // we should fetch it from backend
  inquiries: InquiryDTO[] = []; // we should fetch it
  isProgressSpinnerVisible: boolean = false;
  subscriptions: Subscription[] = [];
  isUserAuthenticated!: boolean;
  applicationDescription: String = applicationDescription;

  constructor(private authenticationHttpService: AuthenticationService,
    private router: Router,
    private homeHttpService: HomeHttpServiceService) {
    this.isUserAuthenticated = this.authenticationHttpService.isUserAuthenticated();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.authenticationHttpService.authenticationStateChanged.subscribe(isAuthenticated => {
        this.isUserAuthenticated = isAuthenticated;
    }));

    this.inquiries = [{id: '1', loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-12-07')},
    {id: '2', loanValue: 1000, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-11-02')},
    {id: '3', loanValue: 109.99, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-12-01')},
    {id: '4', loanValue: 1000, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-11-02')},
    {id: '5', loanValue: 109.99, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-12-01')}]; // we should fetch it here

    if (this.authenticationHttpService.isUserAuthenticated()) {
      const getInquiries$ = this.homeHttpService.getInquiries().pipe(
        tap((inquiries: InquiryDTO[]) => {
          this.inquiries = inquiries;
        })
      );
      this.subscriptions.push(this.doWithLoading(getInquiries$).subscribe());
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  createNewInquiryOnClickHandler(): void {
    this.router.navigate(['create-inquiry']);
  }

  onRowClickHandler(inquiry: InquiryDTO) {
    this.router.navigate([`choose-offer/${inquiry.id}`]);
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }
}
