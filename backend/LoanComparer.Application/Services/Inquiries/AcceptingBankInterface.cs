using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class AcceptingBankInterface : IBankInterface
{
    public string BankName => "AcceptBank";
    
    public Task<SentInquiryStatus> RefreshStatusAsync(SentInquiryStatus status)
    {
        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            LoanValue = status.Inquiry.AmountRequested,
            NumberOfInstallments = status.Inquiry.NumberOfInstallments,
            Percentage = 10,
            MonthlyInstallment = status.Inquiry.AmountRequested * 1.1m / status.Inquiry.NumberOfInstallments
        };
        
        var updatedStatus = new SentInquiryStatus
        {
            Id = status.Id,
            BankName = BankName,
            Inquiry = status.Inquiry,
            ReceivedOffer = offer,
            Status = InquiryStatus.Accepted
        };
        return Task.FromResult(updatedStatus);
    }

    public Task<SentInquiryStatus> SendInquiryAsync(Inquiry inquiry)
    {
        return Task.FromResult(new SentInquiryStatus
        {
            Id = Guid.NewGuid(),
            BankName = BankName,
            Inquiry = inquiry,
            ReceivedOffer = null,
            Status = InquiryStatus.Pending
        });
    }
}