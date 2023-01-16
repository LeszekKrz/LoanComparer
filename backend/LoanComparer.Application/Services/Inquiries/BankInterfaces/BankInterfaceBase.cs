﻿using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Http;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces;

public abstract class BankInterfaceBase : IBankInterface
{
    private readonly IInquiryCommand _inquiryCommand;
    private readonly IOfferCommand _offerCommand;

    protected BankInterfaceBase(IInquiryCommand inquiryCommand, IOfferCommand offerCommand)
    {
        _inquiryCommand = inquiryCommand;
        _offerCommand = offerCommand;
    }
    
    public abstract string BankName { get; }

    protected abstract Task<SentInquiryStatus> GetRefreshedStatusAsync(SentInquiryStatus status);
    
    public virtual async Task<SentInquiryStatus> RefreshStatusAsync(SentInquiryStatus status)
    {
        var newStatus = await GetRefreshedStatusAsync(status);
        if (newStatus.Status == status.Status) return newStatus;

        switch (newStatus.Status)
        {
            case InquiryStatus.Pending:
                throw new InvalidOperationException(
                    $"Inquiry status changed to {InquiryStatus.Pending} from {status.Status}");
            case InquiryStatus.OfferReceived:
                if (newStatus.ReceivedOffer is null)
                    throw new InvalidOperationException(
                        "Inquiry status is marked as offer received, but offer is set to null");
                await _offerCommand.SaveOfferAsync(newStatus.ReceivedOffer);
                await _inquiryCommand.LinkSavedOfferToStatusAsync(newStatus, newStatus.ReceivedOffer.Id);
                break;
            case InquiryStatus.Rejected:
                await _inquiryCommand.MarkAsRejectedAsync(newStatus);
                break;
            case InquiryStatus.Timeout:
                throw new InvalidOperationException();
            case InquiryStatus.Error:
                await _inquiryCommand.MarkAsBankServerErrorAsync(newStatus);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newStatus.Status),
                    $"InquiryStatus {newStatus.Status} is invalid");
        }

        return newStatus;
    }

    public abstract Task<SentInquiryStatus> SendInquiryAsync(Inquiry inquiry);

    public abstract Task<byte[]> GetDocumentContentAsync(Offer offer, SentInquiryStatus sentInquiryStatus);

    public abstract Task ApplyForAnOfferAsync(Offer offer, SentInquiryStatus sentInquiryStatus, IFormFile file);
}