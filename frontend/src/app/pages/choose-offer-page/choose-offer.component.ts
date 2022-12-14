import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { BankOffer } from './models/bank-offer';

@Component({
  selector: 'app-choose-offer',
  templateUrl: './choose-offer.component.html',
  styleUrls: ['./choose-offer.component.scss']
})
export class ChooseOfferComponent implements OnInit {
  inquiryId!: string;
  ourBankOffer!: BankOffer | null;
  otherTeamsBankOffer!: BankOffer | null;
  lecturersBankOffer!: BankOffer | null;


  constructor(private route: ActivatedRoute, private messageService: MessageService) { }

  ngOnInit(): void {
    this.inquiryId = this.route.snapshot.params['inquiryId'];

    this.ourBankOffer = {offerId: 1, loanValue: 100, numberOfInstallments: 20, percentage: 1.45, monthlyInstallment: 5.2, selected: true, signedContract: null};
    this.otherTeamsBankOffer = {offerId: 2, loanValue: 100, numberOfInstallments: 20, percentage: 1.25, monthlyInstallment: 5.1, selected: false, signedContract: null};
    this.lecturersBankOffer = {offerId: 3, loanValue: 100, numberOfInstallments: 20, percentage: 10, monthlyInstallment: 12, selected: false, signedContract: null};
    this.lecturersBankOffer = null;
  }

  handleOnRequestOffer(bankOffer: BankOffer): void {
    console.log(bankOffer);
    // send info to backend and go to home page
  }
}
