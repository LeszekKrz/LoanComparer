import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResetPasswordComponent } from './reset-password.component';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    ResetPasswordComponent
  ],
  exports: [
    ResetPasswordComponent
  ],
  imports: [
    CommonModule,
    CardModule,
    ProgressSpinnerModule,
    ButtonModule,
    InputTextModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class ResetPasswordPageModule { }
