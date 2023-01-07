import { OfferDTO } from "./offer-dto";

export interface BankOfferDTO {
  bank: string;
  status: string;
  offer: OfferDTO | null;
}
