import { HttpResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ConfirmationService, FilterMatchMode, PrimeNGConfig } from 'primeng/api';
import { Table } from 'primeng/table';
import { finalize, forkJoin, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { FileService } from 'src/app/core/services/file.service';
import { InquiryDTO } from './models/inquiry-dto';
import { ApplicationDTO } from './models/application-dto';
import { AdminPanelHttpService } from './services/admin.panel.http.service';
import { ReviewApplicationResponse } from './models/review-application-response.dto';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss']
})
export class AdminPanelComponent implements OnInit, OnDestroy {
  @ViewChild('inquiriesTable') inquiriesTable!: Table;
  @ViewChild('applicationsTable') applicationsTable!: Table;
  inquiries: InquiryDTO[] = [];
  inquiriesCloned: InquiryDTO[] = [];
  applications: ApplicationDTO[] = [];
  applicationsCloned: ApplicationDTO[] = [];
  isProgressSpinnerVisible: boolean = false;
  subscriptions: Subscription[] = [];
  governmentIdTypes: string[] = ['PESEL', 'ID Number', 'Passport Number'];
  statuses: string[] = ['WAITINGFORACCEPTANCE', 'ACCEPTED', 'REJECTED'];

  constructor(private config: PrimeNGConfig,
    private adminPanelHttpService: AdminPanelHttpService,
    private fileService: FileService,
    private confirmationService: ConfirmationService) {}

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

    this.applications = [{id: '1', loanValue: 100, numberOfInstallments: 3, percentage: 10, monthlyInstallment: 43, status: 'WAITINGFORACCEPTANCE', email: 'marcin.latoszek06@gmail.com', dateOfInquiry: new Date("2022-11-04"), dateOfApplication: new Date("2022-11-10"), governmentId: { type: "PESEL", value: "99992123843" }},
    {id: '2', loanValue: 110, numberOfInstallments: 30, percentage: 1, monthlyInstallment: 10, status: 'ACCEPTED', email: 'bilbo@gmail.com', dateOfInquiry: new Date("2022-12-04"), dateOfApplication: new Date("2022-12-10"), governmentId: { type: "PESEL", value: "89328392792"}},
    {id: '3', loanValue: 100, numberOfInstallments: 30, percentage: 1, monthlyInstallment: 10, status: 'ACCEPTED', email: 'bilbo@gmail.com', dateOfInquiry: new Date("2022-12-04"), dateOfApplication: new Date("2022-12-10"), governmentId: { type: "PESEL", value: "89328392792"}},
    {id: '4', loanValue: 300, numberOfInstallments: 34, percentage: 5.5, monthlyInstallment: 43, status: 'REJECTED', email: 'marcin.latoszek06@gmail.com', dateOfInquiry: new Date("2022-11-04"), dateOfApplication: new Date("2022-11-10"), governmentId: { type: "ID NUMBER", value: "33332213940" }}];
    this.applicationsCloned = this.applications.slice(0);

    const getInquiriesAndApplications$ = forkJoin([
      this.adminPanelHttpService.getInquiries().pipe(
        tap((inquiries: InquiryDTO[]) => {
          this.inquiries = inquiries;
        })),
      this.adminPanelHttpService.getOfferRequests().pipe(
        tap((offerRequests: ApplicationDTO[]) => {
          this.applications = offerRequests;
        })
      )
    ]);

    this.subscriptions.push(this.doWithLoading(getInquiriesAndApplications$).subscribe());
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

  clearApplicationsTable(): void {
    this.applicationsTable.clear();
    this.applications = this.applicationsCloned.slice(0);
  }

  downloadContract(offerId: string): void {
    this.subscriptions.push(this.adminPanelHttpService.getContract(offerId).subscribe((data: HttpResponse<Blob>) => {
      this.fileService.downloadResponseContent(data);
    }));
  }

  reviewApplicationPopUp(application: ApplicationDTO, accept: boolean, target: EventTarget): void {
    this.confirmationService.confirm({
      target: target,
      message: accept
        ? 'Are you sure you want to accept this application?'
        : 'Are you sure you want to reject this application?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.subscriptions.push(this.adminPanelHttpService.reviewApplication(application.id, {accept: accept}).subscribe((response: ReviewApplicationResponse) => {
          application.status = response.status;
        }));
        this.applicationsTable._filter();
      }
    });


  }
}
