import { Offer } from "./offer";

export interface BankOffer {
  status: string;
  offer: Offer | null;
  contractDownloaded: boolean;
  signedContract: FormData | null;
}
