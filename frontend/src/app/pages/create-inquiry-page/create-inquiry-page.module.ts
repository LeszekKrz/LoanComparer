import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateInquiryComponent } from './create-inquiry.component';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { InputTextModule } from 'primeng/inputtext'
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { AutoFocusModule } from 'primeng/autofocus';


@NgModule({
  declarations: [
    CreateInquiryComponent
  ],
  exports: [
    CreateInquiryComponent
  ],
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    FormsModule,
    ReactiveFormsModule,
    InputTextModule,
    ProgressSpinnerModule,
    DropdownModule,
    InputNumberModule,
    AutoFocusModule
  ]
})
export class CreateInquiryPageModule { }
