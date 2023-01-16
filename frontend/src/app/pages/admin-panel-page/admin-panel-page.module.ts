import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminPanelComponent } from './admin-panel.component';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { RippleModule } from 'primeng/ripple';
import {ConfirmPopupModule} from 'primeng/confirmpopup';

@NgModule({
  declarations: [
    AdminPanelComponent
  ],
  exports: [
    AdminPanelComponent
  ],
  imports: [
    CommonModule,
    ProgressSpinnerModule,
    TableModule,
    CardModule,
    ButtonModule,
    DropdownModule,
    FormsModule,
    RippleModule,
    ConfirmPopupModule,
  ]
})
export class AdminPanelPageModule { }
