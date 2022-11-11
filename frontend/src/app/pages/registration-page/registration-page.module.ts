import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegistrationComponent } from './registration.component';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown'
import { HttpClientModule } from '@angular/common/http';
import {InputNumberModule} from 'primeng/inputnumber';
import {PasswordModule} from 'primeng/password';


@NgModule({
  declarations: [
    RegistrationComponent
  ],
  exports: [
    RegistrationComponent
  ],
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    FormsModule,
    ReactiveFormsModule,
    InputTextModule,
    DropdownModule,
    HttpClientModule,
    InputNumberModule,
    PasswordModule
  ]
})
export class RegistrationPageModule { }
