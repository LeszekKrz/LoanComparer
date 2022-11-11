import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { CreateInquiryComponent } from "./pages/create-inquiry-page/create-inquiry.component";
import { RegistrationComponent } from "./pages/registration-page/registration.component";


const routes: Routes = [
  {path: 'create-inquiry', component: CreateInquiryComponent},
  {path: 'register', component: RegistrationComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
