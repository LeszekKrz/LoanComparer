import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login.component';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { HttpClientModule } from '@angular/common/http';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [
    LoginComponent
  ],
  exports: [LoginComponent],
  imports: [
    CommonModule,
    ProgressSpinnerModule,
    CardModule,
    FormsModule,
    ReactiveFormsModule,
    InputTextModule,
    HttpClientModule,
    ButtonModule,
    RouterModule
  ]
})
export class LoginPageModule { }
