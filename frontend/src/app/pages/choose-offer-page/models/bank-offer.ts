export interface BankOffer {
    offerId: number; // ewentualnie tu moze byc id inquiry i dajemy info z frontu od kogo jest ta oferta
    loanValue: number;
    numberOfInstallments: number;
    percentage: number;
    monthlyInstallment: number;
    selected: boolean;
    signedContract: FormData | null;
}
