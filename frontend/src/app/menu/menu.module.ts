import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuComponent } from './menu.component';
import { MenubarModule } from 'primeng/menubar';
import { ButtonModule } from 'primeng/button';



@NgModule({
  declarations: [
    MenuComponent
  ],
  exports: [
    MenuComponent
  ],
  imports: [
    CommonModule,
    MenubarModule,
    ButtonModule
  ]
})
export class MenuModule { }
