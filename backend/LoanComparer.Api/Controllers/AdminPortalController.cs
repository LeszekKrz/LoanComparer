using LoanComparer.Application.Configuration;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.InquiryDTO;
using LoanComparer.Application.DTO.OfferApplicationDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using LoanComparer.Application.Services.Inquiries;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;
using LoanComparer.Application.Services.Inquiries.BankInterfaces.This;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = LoanComparerConstants.BankEmployeeRoleName)]
    public class AdminPortalController : ControllerBase
    {
        private readonly IInquiryQuery _inquiryQuery;
        private readonly IOfferQuery _offerQuery;
        private readonly IOfferCommand _offerCommand;
        private readonly IReadOnlyCollection<IBankInterface> _banks;
        private readonly IEmailService _emailService;
        private readonly IOptionsMonitor<InquiryConfiguration> _config;

        public AdminPortalController(
            IInquiryQuery inquiryQuery,
            IOfferQuery offerQuery,
            IOfferCommand offerCommand,
            IBankInterfaceFactory bankInterfaceFactory,
            IEmailService emailService,
            IOptionsMonitor<InquiryConfiguration> config)
        {
            _inquiryQuery = inquiryQuery;
            _offerQuery = offerQuery;
            _offerCommand = offerCommand;
            _banks = bankInterfaceFactory.CreateBankInterfaces();
            _emailService = emailService;
            _config = config;
        }

        [HttpGet("inquiries")]
        public async Task<IReadOnlyCollection<InquiryResponse>> GetAllInquiries()
        {
            return await _inquiryQuery.GetAllInquiries();
        }

        [HttpGet("applications")]
        public async Task<IReadOnlyCollection<OfferApplicationDTO>> GetAllApplications()
        {
            return await _offerQuery.GetAllApplicationsForThisBank();
        }

        [HttpGet("applications/{offerId:guid}/document")]
        public async Task<FileResult> GetSignedDocument(Guid offerId)
        {
            byte[] fileContent = await _offerQuery.GetSignedDocument(offerId);
            return File(fileContent, "text/plain", "signed_contract.txt");
        }

        [HttpPut("applications/{offerId:guid}/review")]
        public async Task<ReviewApplicationResponseDTO> ReviewApplication(Guid offerId, [FromBody] ReviewApplicationRequestDTO reviewApplicationRequest)
        {
            SentInquiryStatus sentInquiryStatus = await _offerQuery.GetStatusWithOfferAndInquiryOrThrowAsync(offerId);
            ThisBankInterface? thisBankInterface = GetThisBankInterface();
            if (thisBankInterface == null)
                throw new InvalidOperationException("This bank interface wasn't found");
            InquiryStatus newInquiryStatus = await thisBankInterface.ReviewApplicationAsync(sentInquiryStatus, reviewApplicationRequest);
            await _offerCommand.UpdateStatusAsync(offerId, newInquiryStatus);
            ApplicationReviewedEmail email = new(
                sentInquiryStatus.Inquiry.PersonalData.NotificationEmail,
                sentInquiryStatus.Inquiry.PersonalData.FirstName,
                newInquiryStatus == InquiryStatus.Accepted,
                string.Format(_config.CurrentValue.CheckInquiryStatusUrl, sentInquiryStatus.Inquiry.Id));
            await _emailService.SendEmailAsync(email, CancellationToken.None);
            return new(newInquiryStatus);
        }

        private ThisBankInterface? GetThisBankInterface()
        {
            return _banks.SingleOrDefault(bank => bank.BankName == LoanComparerConstants.OurBankName) as ThisBankInterface;

        }
    }
}
