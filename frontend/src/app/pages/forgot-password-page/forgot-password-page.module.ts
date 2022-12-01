import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForgotPasswordComponent } from './forgot-password.component';
import { CardModule } from 'primeng/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { InputTextModule } from 'primeng/inputtext';



@NgModule({
  declarations: [
    ForgotPasswordComponent
  ],
  exports: [
    ForgotPasswordComponent
  ],
  imports: [
    CommonModule,
    CardModule,
    ReactiveFormsModule,
    ButtonModule,
    FormsModule,
    ProgressSpinnerModule,
    InputTextModule
  ]
})
export class ForgotPasswordPageModule { }
