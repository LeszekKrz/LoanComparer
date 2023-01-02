using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using Microsoft.AspNetCore.Mvc;

namespace LoanComparer.Api.Controllers;

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
    public async Task<ActionResult> CreateAsync(InquiryRequest request)
    {
        var inquiry = Inquiry.FromRequest(request);
        var statuses = _sender.SendInquiryToAllBanks(inquiry);
        await foreach (var status in statuses)
        {
            await _command.SaveInquiryStatusAsync(status);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InquiryResponse>>> GetAllAsync()
    {
        return (await _query.GetAllAsync()).Select(i => i.ToResponse()).ToList();
    }

    [HttpGet]
    [Route("{inquiryId:guid}/status")]
    public async Task<ActionResult<IReadOnlyList<SentInquiryStatusDTO>>> GetStatusesForInquiryAsync(Guid inquiryId)
    {
        return (await _query.GetStatusesForInquiryAsync(inquiryId)).Select(s => s.ToDto()).ToList();
    }

    // TODO: GET offers
}