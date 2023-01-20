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

    this.inquiriesCloned = this.inquiries.slice(0);
    this.applicationsCloned = this.applications.slice(0);
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
        this.subscriptions.push(this.doWithLoading(this.adminPanelHttpService.reviewApplication(application.offerId, {accept: accept})).subscribe((response: ReviewApplicationResponse) => {
          application.status = response.status;
        }));
        this.applicationsTable._filter();
      }
    });


  }
}
