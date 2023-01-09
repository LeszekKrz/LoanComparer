import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { BankOffer } from './models/bank-offer';
import { BankOfferDTO } from './models/bank-offer-dto';
import { OfferHttpService } from './services/offer.http.service';

@Component({
  selector: 'app-choose-offer',
  templateUrl: './choose-offer.component.html',
  styleUrls: ['./choose-offer.component.scss']
})
export class ChooseOfferComponent implements OnInit, OnDestroy {
  inquiryId!: string;
  ourBankOffer!: BankOffer;
  otherTeamsBankOffer!: BankOffer;
  lecturersBankOffer!: BankOffer;
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;

  constructor(private route: ActivatedRoute,
     private messageService: MessageService,
     private offerHttpService: OfferHttpService) { }

  ngOnInit(): void {
    this.inquiryId = this.route.snapshot.params['inquiryId'];

    this.ourBankOffer = {status: 'ERROR', offer: { id: '1', loanValue: 100, numberOfInstallments: 20, percentage: 1.45, monthlyInstallment: 5.2}, contractDownloaded: false, signedContract: null};
    this.otherTeamsBankOffer = {status: 'OFFERRECEIVED', offer: { id: '1', loanValue: 100, numberOfInstallments: 20, percentage: 1.45, monthlyInstallment: 5.2}, contractDownloaded: false, signedContract: null};
    this.lecturersBankOffer = {status: 'SUBMITTED', offer: { id: '1', loanValue: 100, numberOfInstallments: 20, percentage: 1.45, monthlyInstallment: 5.2}, contractDownloaded: false, signedContract: null};

    const getOffers$ = this.offerHttpService.getOffers(this.route.snapshot.params['inquiryId']).pipe(
      tap((bankOffersDTO: BankOfferDTO[]) => {
        bankOffersDTO.forEach(bankOfferDTO => {
          const bankOffer: BankOffer = this.getBankOfferFromDTO(bankOfferDTO);
          if (bankOfferDTO.bank == 'our') {
            this.ourBankOffer = bankOffer;
          }
          else if (bankOfferDTO.bank == 'lecturer') {
            this.lecturersBankOffer = bankOffer;
          }
          else if (bankOfferDTO.bank == 'other team') {
            this.otherTeamsBankOffer = bankOffer;
          }
        });
      }),
    );
    this.subscriptions.push(this.doWithLoading(getOffers$).subscribe());
  }

  private getBankOfferFromDTO(bankOfferDTO: BankOfferDTO): BankOffer {
    return {
      status: bankOfferDTO.status,
      offer: bankOfferDTO.offer == null
        ? null
        : {
          id: bankOfferDTO.offer.id,
          loanValue: bankOfferDTO.offer.loanValue,
          numberOfInstallments: bankOfferDTO.offer.numberOfInstallments,
          percentage: bankOfferDTO.offer.percentage,
          monthlyInstallment: bankOfferDTO.offer.monthlyInstallments,
        },
      contractDownloaded: false,
      signedContract: null,
    };
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  handleOnOfferRequest(bankOffer: BankOffer): void {
    console.log(bankOffer);
    // send info to backend and go to home page
  }
}