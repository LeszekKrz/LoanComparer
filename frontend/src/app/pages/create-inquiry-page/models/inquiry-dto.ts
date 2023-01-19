import { GovernmentIdDTO } from "./government-id-dto";
import { JobDetailsDTO } from "./job-details-dto";
import { PersonalDataDTO } from "./personal-data-dto";

export interface InquiryDTO {
  amountRequested: number;
  numberOfInstallments: number;
  personalData: PersonalDataDTO;
  governmentId: GovernmentIdDTO;
  jobDetails: JobDetailsDTO;
}
