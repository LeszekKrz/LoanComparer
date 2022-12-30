using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class InquirySender : IInquirySender
{
    private readonly IReadOnlyList<IBankApiSender> _senders;

    public InquirySender(IEnumerable<IBankApiSender> senders)
    {
        _senders = senders.ToList();
    }

    public async IAsyncEnumerable<SentInquiryStatus> SendInquiryToAllBanks(Inquiry inquiry)
    {
        foreach (var sender in _senders)
        {
            yield return await sender.SendInquiryAsync(inquiry);
        }
    }
}