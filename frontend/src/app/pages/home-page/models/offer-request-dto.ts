import { InquiryDTO } from "./inquiry-dto";

export interface OfferRequest {
  id: string;
  inquiry: InquiryDTO;
  loanValue: number;
  numberOfInstallments: number;
  status: string;
}
