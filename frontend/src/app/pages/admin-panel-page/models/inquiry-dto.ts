import { GovernmentIdDTO } from "./government-id-dto";
import { JobDetailsDTO } from "./job-details-dto";
import { PersonalDataDTO } from "./personal-data-dto";

export interface InquiryDTO {
  id: string;
  amountRequested: number;
  numberOfInstallments: number;
  creationTime: Date;
  personalData: PersonalDataDTO;
  govId: GovernmentIdDTO;
  jobDetails: JobDetailsDTO;
}
