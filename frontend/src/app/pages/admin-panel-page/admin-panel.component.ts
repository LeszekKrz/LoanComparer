import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FilterMatchMode, PrimeNGConfig } from 'primeng/api';
import { Table } from 'primeng/table';
import { finalize, Observable, of, Subscription, switchMap } from 'rxjs';
import { InquiryWithUserInfo } from './models/inquiry-with-user-info';
import { OfferRequestWithUserInfo } from './models/offer-request-with-user-info';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss']
})
export class AdminPanelComponent implements OnInit, OnDestroy {
  @ViewChild('inquiriesTable') inquiriesTable!: Table;
  @ViewChild('offerRequestsTable') offerRequestsTable!: Table;
  inquiriesWithUserInfo: InquiryWithUserInfo[] = []; // we should fetch it
  inquiriesWithUserInfoCloned: InquiryWithUserInfo[] = [];
  offerRequestsWithUserInfo: OfferRequestWithUserInfo[] = [];
  offerRequestsWithUserInfoCloned: OfferRequestWithUserInfo[] = [];
  isProgressSpinnerVisible: boolean = false;
  subscriptions: Subscription[] = [];
  governmentIdTypes: string[] = ['PESEL', 'ID Number', 'Passport Number'];
  statuses: string[] = ['WAITINGFORACCEPTANCE', 'ACCEPTED', 'REJECTED'];

  constructor(private config: PrimeNGConfig) {}

  ngOnInit(): void {
    this.config.filterMatchModeOptions = {
      text: [
        FilterMatchMode.STARTS_WITH,
        FilterMatchMode.CONTAINS,
        FilterMatchMode.ENDS_WITH,
        FilterMatchMode.EQUALS
    ],
    numeric: [
        FilterMatchMode.EQUALS,
        FilterMatchMode.LESS_THAN,
        FilterMatchMode.LESS_THAN_OR_EQUAL_TO,
        FilterMatchMode.GREATER_THAN,
        FilterMatchMode.GREATER_THAN_OR_EQUAL_TO
    ],
    date: [
        FilterMatchMode.DATE_IS,
        FilterMatchMode.DATE_IS_NOT,
        FilterMatchMode.DATE_BEFORE,
        FilterMatchMode.DATE_AFTER
    ]
    }

    this.inquiriesWithUserInfo = [{loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 34453, numberOfInstallments: 25, dateOfInquirySubmition: new Date("2022-12-09"), email: "bilbo@gmail.com", governmentIdType: "ID", governmentIdValue: "DAB888392"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
    {loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date("2022-12-07"), email: "marcin.latoszek06@gmail.com", governmentIdType: "PESEL", governmentIdValue: "01222709824"},
  ]; // we should fetch it
    this.inquiriesWithUserInfoCloned = this.inquiriesWithUserInfo.slice(0);

    this.offerRequestsWithUserInfo = [{id: '1', loanValue: 100, numberOfInstallments: 3, percentage: 10, monthlyInstallment: 43, status: 'WAITINGFORACCEPTANCE', email: 'marcin.latoszek06@gmail.com', dateOfInquirySubmition: new Date("2022-11-04"), dateOfOfferRequest: new Date("2022-11-10"), governmentIdType: "PESEL", governmentIdValue: "99992123843"},
    {id: '2', loanValue: 110, numberOfInstallments: 30, percentage: 1, monthlyInstallment: 10, status: 'ACCEPTED', email: 'bilbo@gmail.com', dateOfInquirySubmition: new Date("2022-12-04"), dateOfOfferRequest: new Date("2022-12-10"), governmentIdType: "PESEL", governmentIdValue: "89328392792"},
    {id: '3', loanValue: 100, numberOfInstallments: 30, percentage: 1, monthlyInstallment: 10, status: 'ACCEPTED', email: 'bilbo@gmail.com', dateOfInquirySubmition: new Date("2022-12-04"), dateOfOfferRequest: new Date("2022-12-10"), governmentIdType: "PESEL", governmentIdValue: "89328392792"},
    {id: '4', loanValue: 300, numberOfInstallments: 34, percentage: 5.5, monthlyInstallment: 43, status: 'REJECTED', email: 'marcin.latoszek06@gmail.com', dateOfInquirySubmition: new Date("2022-11-04"), dateOfOfferRequest: new Date("2022-11-10"), governmentIdType: "ID NUMBER", governmentIdValue: "33332213940"}];
    this.offerRequestsWithUserInfoCloned = this.offerRequestsWithUserInfo.slice(0);
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

  clearInquiriesTable(): void {
    this.inquiriesTable.clear();
    this.inquiriesWithUserInfo = this.inquiriesWithUserInfoCloned.slice(0);
  }

  clearOfferRequestsTable(): void {
    this.offerRequestsTable.clear();
    this.offerRequestsWithUserInfo = this.offerRequestsWithUserInfoCloned.slice(0);
  }
}
