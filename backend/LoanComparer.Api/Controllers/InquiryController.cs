using System.Security.Claims;
using FluentValidation;
using LoanComparer.Application.Configuration;
using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using LoanComparer.Application.Services.Inquiries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoanComparer.Api.Controllers;

[ApiController]
[Route("api/inquiries")]
public sealed class InquiryController : ControllerBase
{
    private readonly IInquiryQuery _query;
    private readonly IInquiryCommand _command;
    private readonly IInquirySender _sender;
    private readonly IInquiryRefresher _refresher;
    private readonly IValidator<InquiryRequest> _validator;
    private readonly IEmailService _emailService;
    private readonly IOptionsMonitor<InquiryConfiguration> _config;

    public InquiryController(
        IInquiryQuery query,
        IInquiryCommand command,
        IInquirySender sender,
        IInquiryRefresher refresher,
        IValidator<InquiryRequest> validator,
        IEmailService emailService,
        IOptionsMonitor<InquiryConfiguration> config)
    {
        _query = query;
        _command = command;
        _sender = sender;
        _refresher = refresher;
        _validator = validator;
        _emailService = emailService;
        _config = config;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<InquiryResponse>> CreateAsync(InquiryRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        string? username = GetUsername();
        var inquiry = Inquiry.FromRequest(request, username);
        var statuses = _sender.SendInquiryToAllBanks(inquiry);
        await foreach (var status in statuses)
        {
            await _command.SaveInquiryStatusAsync(status);
        }

        if (username == null)
        {
            NewInquiryEmail email = new(
                inquiry.PersonalData.NotificationEmail,
                inquiry.PersonalData.FirstName,
                string.Format(_config.CurrentValue.CheckInquiryStatusUrl, inquiry.Id));
            await _emailService.SendEmailAsync(email, CancellationToken.None);
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
    [Route("{inquiryId:guid}/offers")]
    public async Task<ActionResult<IReadOnlyList<SentInquiryStatusDTO>>> GetStatusesForInquiryAsync(Guid inquiryId)
    {
        var checkResult = await _query.CheckOwnerAsync(inquiryId, GetUsername());
        if (checkResult == OwnershipTestResult.DoesNotExist)
            return BadRequest();
        if (checkResult == OwnershipTestResult.Unauthorized)
            return Unauthorized();

        await _refresher.RefreshStatusesForInquiryAsync(inquiryId);
        return (await _query.GetStatusesForInquiryAsync(inquiryId)).Select(s => s.ToDto()).ToList();
    }

    private string? GetUsername()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value; 
    }
}