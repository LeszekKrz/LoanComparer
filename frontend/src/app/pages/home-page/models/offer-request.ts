import { Inquiry } from "./inquiry";

export interface OfferRequest {
  id: string;
  inquiry: Inquiry;
  loanValue: number;
  numberOfInstallments: number;
  status: string;
}
