using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IBankApiSender
{
    Guid BankId { get; }

    SentInquiryStatus SendInquiry(Inquiry inquiry);
}