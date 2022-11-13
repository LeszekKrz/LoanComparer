import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminPanelComponent } from './admin-panel.component';



@NgModule({
  declarations: [
    AdminPanelComponent
  ],
  exports: [
    AdminPanelComponent
  ],
  imports: [
    CommonModule
  ]
})
export class AdminPanelPageModule { }
