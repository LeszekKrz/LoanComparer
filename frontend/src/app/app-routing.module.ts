import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { CreateInquiryComponent } from "./pages/create-inquiry-page/create-inquiry.component";


const routes: Routes = [
  {path: 'create-inquiry', component: CreateInquiryComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
