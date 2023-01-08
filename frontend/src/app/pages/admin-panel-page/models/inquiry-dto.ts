import { GovernmentIdDTO } from "./government-id-dto";
import { JobDetailsDTO } from "./job-details-dto";
import { PersonalDataDTO } from "./personal-data-dto";

export interface InquiryDTO {
  id: string;
  loanValue: number;
  numberOfInstallments: number;
  dateOfInquirySubmition: Date;
  personalData: PersonalDataDTO;
  governmentId: GovernmentIdDTO;
  jobDetails: JobDetailsDTO;
}
