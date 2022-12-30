using LoanComparer.Application.DTO;
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
    public async Task<ActionResult> CreateAsync(InquiryDTO request)
    {
        var inquiry = Inquiry.FromDto(request);
        var statuses = _sender.SendInquiryToAllBanks(inquiry);
        await foreach (var status in statuses)
        {
            _command.SaveInquiryStatus(status);
        }

        return Ok();
    }
    
    // GETs...
}