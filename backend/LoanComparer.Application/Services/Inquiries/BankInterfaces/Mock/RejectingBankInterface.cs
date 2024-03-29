﻿using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces.Mock;

public sealed class RejectingBankInterface : BankInterfaceBase
{
    public RejectingBankInterface(IInquiryCommand inquiryCommand, IOfferCommand offerCommand) : base(inquiryCommand,
        offerCommand)
    {
    }
    
    public override string BankName => "RejectBank";
    
    protected override Task<SentInquiryStatus> GetRefreshedStatusAsync(SentInquiryStatus status)
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

    public override Task<SentInquiryStatus> SendInquiryAsync(Inquiry inquiry)
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

    public override Task<Stream> GetDocumentContentAsync(SentInquiryStatus sentInquiryStatus)
    {
        return Task.FromResult(Stream.Null);
    }

    public override Task<InquiryStatus> ApplyForAnOfferAsync(SentInquiryStatus sentInquiryStatus, IFormFile file)
    {
        return Task.FromResult(InquiryStatus.WaitingForAcceptance);
    }
}