import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { BankOffer } from '../models/bank-offer';

@Component({
  selector: 'app-offer-card',
  templateUrl: './offer-card.component.html',
  styleUrls: ['./offer-card.component.scss']
})
export class OfferCardComponent {
  @Input() offerName!: string;
  @Input() bankOffer!: BankOffer | null;
  @Output() onRequestOffer: EventEmitter<BankOffer> = new EventEmitter();
  @ViewChild('fileUpload') fileUpload!: FileUpload;
  @ViewChild('fileInput') fileInput!: HTMLElement;

  constructor(private messageService: MessageService) {}

  selectOffer(): void {
    this.bankOffer!.selected = true;
  }

  onSelectFileHandler(event: any): void {
    console.log(event.type)
    console.log(event.files[0]);
    const signedContract = event.files[0];
    if (signedContract.type == 'text/plain') {
      this.bankOffer!.signedContract = new FormData();
      this.bankOffer!.signedContract.append('signedContract', new Blob([signedContract], {type: 'txt'}), signedContract.name);
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

    this.onRequestOffer.emit(this.bankOffer!);
  }

  downloadContract(): void {
    // code here
  }
}
