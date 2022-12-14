export interface OfferRequestWithUserInfo {
    id: string;
    loanValue: number;
    numberOfInstallments: number;
    percentage: number;
    monthlyInstallment: number;
    status: string;
    email: string;
    dateOfInquirySubmition: Date;
    dateOfOfferRequest: Date;
    governmentIdType: string;
    governmentIdValue: string;
}
