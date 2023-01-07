import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FilterMatchMode, PrimeNGConfig } from 'primeng/api';
import { Table } from 'primeng/table';
import { finalize, Observable, of, Subscription, switchMap } from 'rxjs';
import { InquiryDTO } from './models/inquiry-dto';
import { OfferRequestDTO } from './models/offer-request-dto';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss']
})
export class AdminPanelComponent implements OnInit, OnDestroy {
  @ViewChild('inquiriesTable') inquiriesTable!: Table;
  @ViewChild('offerRequestsTable') offerRequestsTable!: Table;
  inquiries: InquiryDTO[] = [];
  inquiriesCloned: InquiryDTO[] = [];
  offerRequestsWithUserInfo: OfferRequestDTO[] = [];
  offerRequestsWithUserInfoCloned: OfferRequestDTO[] = [];
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

    this.inquiries = [{id: '1', loanValue: 100, numberOfInstallments: 5, dateOfInquirySubmition: new Date('2022-12-07'), personalData: {email: 'bilbo@gmail.com', firstName: 'Bilbo', lastName: 'Baggins', birthDate: null}, governmentId: {type: 'PESEL', value: '11122233312'}, jobDetails: {name: 'Nikt', incomeLevel: 1000, description: null, startDate: null, endDate: null}}];
    this.inquiriesCloned = this.inquiries.slice(0);

    this.offerRequestsWithUserInfo = [{id: '1', loanValue: 100, numberOfInstallments: 3, percentage: 10, monthlyInstallment: 43, status: 'WAITINGFORACCEPTANCE', email: 'marcin.latoszek06@gmail.com', dateOfInquiry: new Date("2022-11-04"), dateOfApplication: new Date("2022-11-10"), governmentId: { type: "PESEL", value: "99992123843" }},
    {id: '2', loanValue: 110, numberOfInstallments: 30, percentage: 1, monthlyInstallment: 10, status: 'ACCEPTED', email: 'bilbo@gmail.com', dateOfInquiry: new Date("2022-12-04"), dateOfApplication: new Date("2022-12-10"), governmentId: { type: "PESEL", value: "89328392792"}},
    {id: '3', loanValue: 100, numberOfInstallments: 30, percentage: 1, monthlyInstallment: 10, status: 'ACCEPTED', email: 'bilbo@gmail.com', dateOfInquiry: new Date("2022-12-04"), dateOfApplication: new Date("2022-12-10"), governmentId: { type: "PESEL", value: "89328392792"}},
    {id: '4', loanValue: 300, numberOfInstallments: 34, percentage: 5.5, monthlyInstallment: 43, status: 'REJECTED', email: 'marcin.latoszek06@gmail.com', dateOfInquiry: new Date("2022-11-04"), dateOfApplication: new Date("2022-11-10"), governmentId: { type: "ID NUMBER", value: "33332213940" }}];
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
    this.inquiries = this.inquiriesCloned.slice(0);
  }

  clearOfferRequestsTable(): void {
    this.offerRequestsTable.clear();
    this.offerRequestsWithUserInfo = this.offerRequestsWithUserInfoCloned.slice(0);
  }
}
