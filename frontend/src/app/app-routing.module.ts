import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { BankEmployeeGuard } from "./authentication/guards/bank.employee.guard";
import { AdminPanelComponent } from "./pages/admin-panel-page/admin-panel.component";
import { ChooseOfferComponent } from "./pages/choose-offer-page/choose-offer.component";
import { ConfirmEmailComponent } from "./pages/confirm-email-page/confirm-email.component";
import { CreateInquiryComponent } from "./pages/create-inquiry-page/create-inquiry.component";
import { ForgotPasswordComponent } from "./pages/forgot-password-page/forgot-password.component";
import { HomeComponent } from "./pages/home-page/home.component";
import { LoginComponent } from "./pages/login-page/login.component";
import { RegistrationComponent } from "./pages/registration-page/registration.component";
import { ResetPasswordComponent } from "./pages/reset-password-page/reset-password.component";


const routes: Routes = [
  {path: 'home', component: HomeComponent},
  {path: 'create-inquiry', component: CreateInquiryComponent},
  {path: 'register', component: RegistrationComponent},
  {path: 'login', component: LoginComponent},
  {path: 'admin-panel', component: AdminPanelComponent, canActivate: [BankEmployeeGuard]},
  {path: 'forgot-password', component: ForgotPasswordComponent},
  {path: 'reset-password', component: ResetPasswordComponent},
  {path: 'confirm-email', component: ConfirmEmailComponent},
  {path: 'choose-offer/:inquiryId', component: ChooseOfferComponent},
  {path: '**', component: HomeComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
