import { OfferDTO } from "./offer-dto";

export interface BankOfferDTO {
  bankName: string;
  status: string;
  offer: OfferDTO | null;
}
