using EllipticCurve.Utils;
using LoanComparer.Application.DTO.OfferDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferQuery _query;
        private readonly IOfferCommand _command;
        private readonly IReadOnlyCollection<IBankInterface> _banks;

        public OfferController(IOfferQuery offerQuery, IOfferCommand offerCommand, IBankInterfaceFactory bankInterfaceFactory)
        {
            _query = offerQuery;
            _command = offerCommand;
            _banks = bankInterfaceFactory.CreateBankInterfaces();
        }

        [HttpGet]
        [Route("offers/{offerId:guid}/document")]
        public async Task<ActionResult> GetContractAsync(Guid offerId)
        {
            var checkResult = await _query.CheckOwnerAsync(offerId, GetUsername());
            if (checkResult == OwnershipTestResult.DoesNotExist)
                return BadRequest();
            if (checkResult == OwnershipTestResult.Unauthorized)
                return Unauthorized();

            OfferEntity offerEntity = await _query.GetOfferEntityWithStatusOrThrow(offerId);
            var bank = GetBankInterfaceOrThrow(offerEntity.SentInquiryStatus.BankName);
            byte[] fileContent = await bank.GetDocumentContentAsync(offerEntity);
            var fileName = "contract.txt";
            SetContentDispositionHeader(fileName);
            return File(fileContent, "text/plain", fileName);
        }

        private void SetContentDispositionHeader(string fileName)
        {
            ContentDisposition contentDisposition = new()
            {
                FileName = fileName,
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
        }

        [HttpPost]
        [Route("offers/{offerId:guid}/apply")]
        public async Task<ActionResult<ApplyForAnOfferResponse>> ApplyForAnOfferAsync(Guid offerId, IFormFile formFile)
        {
            var checkResult = await _query.CheckOwnerAsync(offerId, GetUsername());
            if (checkResult == OwnershipTestResult.DoesNotExist)
                return BadRequest();
            if (checkResult == OwnershipTestResult.Unauthorized)
                return Unauthorized();
            OfferEntity offerEntity = await _query.GetOfferEntityWithStatusOrThrow(offerId);
            var bank = GetBankInterfaceOrThrow(offerEntity.SentInquiryStatus.BankName);
            InquiryStatus updatedStatus = await bank.ApplyForAnOfferAsync(offerEntity, formFile);
            await _command.SetStatusOfAnOfferAsync(offerId, updatedStatus);
            return ApplyForAnOfferResponse.FromInquiryStatus(updatedStatus);
        }

        private string? GetUsername()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }

        private IBankInterface GetBankInterfaceOrThrow(string bankName)
        {
            IBankInterface? bank = _banks.SingleOrDefault(r => r.BankName == bankName);
            if (bank is null)
                throw new InvalidOperationException(
                    $"There is no known bank with name {bankName}");
            return bank;
        }
    }
}
