namespace LoanComparer.Application.DTO.OfferDTO;

public sealed class OfferWithBankName
{
    public string BankName { get; init; } = null!;

    public OfferResponse Offer { get; init; } = null!;
}