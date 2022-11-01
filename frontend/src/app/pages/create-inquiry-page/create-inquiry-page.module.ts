import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateInquiryComponent } from './create-inquiry.component';



@NgModule({
  declarations: [
    CreateInquiryComponent
  ],
  exports: [
    CreateInquiryComponent
  ],
  imports: [
    CommonModule
  ]
})
export class CreateInquiryPageModule { }
