using LoanComparer.Application.DTO.OfferDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferQuery _query;
        private readonly IOfferCommand _command;

        public OfferController(IOfferQuery offerQuery, IOfferCommand offerCommand)
        {
            _query = offerQuery;
            _command = offerCommand;
        }

        [HttpGet]
        [Route("offers/{offerId:guid}/document")]
        public async Task<ActionResult<FileResult>> GetContractAsync(Guid offerId)
        {
            var checkResult = await _query.CheckOwnerAsync(offerId, GetUsername());
            if (checkResult == OwnershipTestResult.DoesNotExist)
                return BadRequest();
            if (checkResult == OwnershipTestResult.Unauthorized)
                return Unauthorized();
            byte[] fileContent = await _query.GetDocumentContentAsync(offerId);
            return File(fileContent, "text/txt", "contract.txt");
        }

        [HttpPost]
        [Route("offers/{offerId:guid}/apply")]
        public async Task<ActionResult<ApplyForAnOfferResponse>> ApplyForAnOfferAsync(Guid offerId, [FromBody] IFormFile formFile)
        {
            var checkResult = await _query.CheckOwnerAsync(offerId, GetUsername());
            if (checkResult == OwnershipTestResult.DoesNotExist)
                return BadRequest();
            if (checkResult == OwnershipTestResult.Unauthorized)
                return Unauthorized();
            InquiryStatus updatedStatus = await _command.ApplyForAnOfferAsync(offerId, formFile);
            await _command.SetStatusOfAnOfferAsync(offerId, updatedStatus);
            return ApplyForAnOfferResponse.FromInquiryStatus(updatedStatus);
        }

        private string? GetUsername()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
    }
}
