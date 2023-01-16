using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces.Mock;

public sealed class AcceptingBankInterface : BankInterfaceBase
{
    public AcceptingBankInterface(IInquiryCommand inquiryCommand, IOfferCommand offerCommand) : base(inquiryCommand,
        offerCommand)
    {
    }
    
    public override string BankName => "AcceptBank";
    
    protected override Task<SentInquiryStatus> GetRefreshedStatusAsync(SentInquiryStatus status)
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
            Status = InquiryStatus.OfferReceived
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

    public override Task ApplyForAnOfferAsync(SentInquiryStatus sentInquiryStatus, IFormFile file)
    {
        return Task.CompletedTask;
    }
}