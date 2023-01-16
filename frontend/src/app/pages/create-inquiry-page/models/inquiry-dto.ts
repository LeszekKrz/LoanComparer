import { GovernmentIdDTO } from "./government-id-dto";
import { JobDetailsDTO } from "./job-details-dto";
import { PersonalDataDTO } from "./personal-data-dto";

export interface InquiryDTO {
  loanValue: number;
  numberOfInstallments: number;
  personalDataDTO: PersonalDataDTO;
  governmentIdDTO: GovernmentIdDTO;
  jobDetailsDTO: JobDetailsDTO;
}
