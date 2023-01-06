using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class InquirySender : IInquirySender
{
    private readonly IReadOnlyList<IBankInterface> _bankInterfaces;

    public InquirySender(IBankInterfaceCreator bankInterfaceCreator)
    {
        _bankInterfaces = bankInterfaceCreator.CreateBankInterfaces();
    }

    public async IAsyncEnumerable<SentInquiryStatus> SendInquiryToAllBanks(Inquiry inquiry)
    {
        foreach (var sender in _bankInterfaces)
        {
            yield return await sender.SendInquiryAsync(inquiry);
        }
    }
}