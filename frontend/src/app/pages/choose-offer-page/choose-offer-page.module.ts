import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChooseOfferComponent } from './choose-offer.component';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { FileUploadModule } from 'primeng/fileupload';
import { RippleModule } from 'primeng/ripple';
import { OfferCardComponent } from './components/offer-card.component';



@NgModule({
  declarations: [
    ChooseOfferComponent,
    OfferCardComponent
  ],
  exports: [
    ChooseOfferComponent
  ],
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    TableModule,
    FileUploadModule,
    RippleModule
  ]
})
export class ChooseOfferPageModule { }
