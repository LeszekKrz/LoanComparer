using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class RejectingBankInterface : IBankInterface
{
    public string BankName => "RejectBank";
    
    public Task<SentInquiryStatus> RefreshStatusAsync(SentInquiryStatus status)
    {
        var updatedStatus = new SentInquiryStatus
        {
            Id = status.Id,
            BankName = BankName,
            Inquiry = status.Inquiry,
            ReceivedOffer = status.ReceivedOffer,
            Status = InquiryStatus.Rejected
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