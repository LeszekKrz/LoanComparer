using System.Security.Claims;
using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.DTO.OfferDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers;

// TODO: Check if user exists
[ApiController]
[Route("api/inquiries")]
public sealed class InquiryController : ControllerBase
{
    private readonly IInquiryQuery _query;
    private readonly IInquiryCommand _command;
    private readonly IInquirySender _sender;

    public InquiryController(IInquiryQuery query, IInquiryCommand command, IInquirySender sender)
    {
        _query = query;
        _command = command;
        _sender = sender;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<InquiryResponse>> CreateAsync(InquiryRequest request)
    {
        var inquiry = Inquiry.FromRequest(request, GetUsername());
        var statuses = _sender.SendInquiryToAllBanks(inquiry);
        await foreach (var status in statuses)
        {
            await _command.SaveInquiryStatusAsync(status);
        }

        return inquiry.ToResponse();
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InquiryResponse>>> GetAllAsync()
    {
        var username = GetUsername();
        if (username is null)
        {
            return Unauthorized();
        }
        
        return (await _query.GetAllForUserAsync(username)).Select(i => i.ToResponse()).ToList();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{inquiryId:guid}/status")]
    public async Task<ActionResult<IReadOnlyList<SentInquiryStatusDTO>>> GetStatusesForInquiryAsync(Guid inquiryId)
    {
        var checkResult = await _query.CheckOwnerAsync(inquiryId, GetUsername());
        return checkResult switch
        {
            OwnershipTestResult.DoesNotExist => BadRequest(),
            OwnershipTestResult.Unauthorized => Unauthorized(),
            _ => (await _query.GetStatusesForInquiryAsync(inquiryId)).Select(s => s.ToDto()).ToList()
        };
    }

    [HttpGet]
    [Route("{inquiryId:guid}/offers")]
    public async Task<ActionResult<IReadOnlyList<OfferWithBankName>>> GetOffersForInquiry(Guid inquiryId)
    {
        var checkResult = await _query.CheckOwnerAsync(inquiryId, GetUsername());
        return checkResult switch
        {
            OwnershipTestResult.DoesNotExist => BadRequest(),
            OwnershipTestResult.Unauthorized => Unauthorized(),
            _ => (await _query.GetStatusesForInquiryAsync(inquiryId)).
                Select(OfferWithBankName.FromSentInquiryStatus).
                Where(o => o is not null).
                Select(o => o!).
                ToList()
        };
    }

    private string? GetUsername()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;   
    }
}