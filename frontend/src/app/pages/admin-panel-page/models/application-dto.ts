import { GovernmentIdDTO } from "./government-id-dto";

export interface ApplicationDTO {
    offerId: string;
    loanValue: number;
    numberOfInstallments: number;
    percentage: number;
    monthlyInstallment: number;
    status: string;
    email: string;
    dateOfInquiry: Date;
    dateOfApplication: Date;
    governmentId: GovernmentIdDTO;
}
