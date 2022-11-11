import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { ToastModule } from 'primeng/toast'
import { AppComponent } from './app.component';
import { CreateInquiryPageModule } from './pages/create-inquiry-page/create-inquiry-page.module';
import { CoreModule } from './core/core.module';
import { RegistrationPageModule } from './pages/registration-page/registration-page.module';
import { MessageService } from 'primeng/api';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ToastModule,
    CreateInquiryPageModule,
    BrowserAnimationsModule,
    CoreModule,
    RegistrationPageModule
  ],
  providers: [MessageService],
  bootstrap: [AppComponent]
})
export class AppModule { }
