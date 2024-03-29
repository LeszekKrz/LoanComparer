﻿using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class InquirySender : IInquirySender
{
    private readonly IReadOnlyList<IBankInterface> _bankInterfaces;

    public InquirySender(IBankInterfaceFactory bankInterfaceFactory)
    {
        _bankInterfaces = bankInterfaceFactory.CreateBankInterfaces();
    }

    public async IAsyncEnumerable<SentInquiryStatus> SendInquiryToAllBanks(Inquiry inquiry)
    {
        foreach (var sender in _bankInterfaces)
        {
            yield return await sender.SendInquiryAsync(inquiry);
        }
    }
}