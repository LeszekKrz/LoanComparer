import { HttpResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnDestroy, Output, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { Subscription } from 'rxjs';
import { FileService } from 'src/app/core/services/file.service';
import { BankOffer } from '../models/bank-offer';
import { OfferHttpService } from '../services/offer.http.service';

@Component({
  selector: 'app-offer-card',
  templateUrl: './offer-card.component.html',
  styleUrls: ['./offer-card.component.scss']
})
export class OfferCardComponent implements OnDestroy {
  @Input() offerName!: string;
  @Input() bankOffer!: BankOffer;
  @Output() onOfferRequest: EventEmitter<BankOffer> = new EventEmitter();
  @ViewChild('fileUpload') fileUpload!: FileUpload;
  @ViewChild('fileInput') fileInput!: HTMLElement;
  subscriptions: Subscription[] = [];

  constructor(
    private messageService: MessageService,
    private offerHttpService: OfferHttpService,
    private fileService: FileService) {}

  onSelectFileHandler(event: any): void {
    console.log(event.type)
    console.log(event.files[0]);
    const signedContract = event.files[0];
    if (signedContract.type == 'text/plain') {
      this.bankOffer!.signedContract = new FormData();
      this.bankOffer!.signedContract.append('formFile', new Blob([signedContract], {type: 'txt'}), signedContract.name);
    }
  }

  onRemoveFileHandler(): void {
    this.bankOffer!.signedContract = null;
    this.fileUpload.clear();
  }

  requestOfferOnClickHandeler(): void {
    if (this.bankOffer!.signedContract == null) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Please upload signed contract'
      });
      return;
    }

    this.onOfferRequest.emit(this.bankOffer!);
  }

  downloadContract(): void {
    if (this.bankOffer.offer == null) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: `Can't download contract`
      });
      return;
    }

    this.bankOffer.contractDownloaded = true;
    this.subscriptions.push(this.offerHttpService.getContract(this.bankOffer.offer.id).subscribe((data: HttpResponse<Blob>) => {
      this.fileService.downloadResponseContent(data);
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }
}
