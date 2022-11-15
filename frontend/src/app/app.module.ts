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
import { LoginPageModule } from './pages/login-page/login-page.module';
import { MenuModule } from './menu/menu.module';
import { JwtModule } from '@auth0/angular-jwt';
import { AdminPanelPageModule } from './pages/admin-panel-page/admin-panel-page.module';
import { HomePageModule } from './pages/home-page/home-page.module';
import { ForgotPasswordPageModule } from './pages/forgot-password-page/forgot-password-page.module';
import { ResetPasswordPageModule } from './pages/reset-password-page/reset-password-page.module';

export function tokenGetter(): string | null {
  return localStorage.getItem('token');
}

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
    RegistrationPageModule,
    LoginPageModule,
    MenuModule,
    AdminPanelPageModule,
    HomePageModule,
    ForgotPasswordPageModule,
    ResetPasswordPageModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:7282'],
        disallowedRoutes: []
      }
    })
  ],
  providers: [MessageService],
  bootstrap: [AppComponent]
})
export class AppModule { }
