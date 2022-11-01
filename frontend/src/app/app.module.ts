import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { ToastModule } from 'primeng/toast'
import { AppComponent } from './app.component';
import { CreateInquiryPageModule } from './pages/create-inquiry-page/create-inquiry-page.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ToastModule,
    CreateInquiryPageModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
