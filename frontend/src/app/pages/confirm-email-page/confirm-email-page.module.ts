import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmEmailComponent } from './confirm-email.component';
import { RouterModule } from '@angular/router';
import { ProgressSpinnerModule } from 'primeng/progressspinner';



@NgModule({
  declarations: [
    ConfirmEmailComponent
  ],
  exports: [
    ConfirmEmailComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    ProgressSpinnerModule
  ]
})
export class ConfirmEmailPageModule { }
