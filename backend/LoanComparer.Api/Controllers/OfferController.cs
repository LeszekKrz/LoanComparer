﻿using LoanComparer.Application.DTO.OfferDTO;
using LoanComparer.Application.Exceptions;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Inquiries;
using LoanComparer.Application.Services.Inquiries.BankInterfaces;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;

namespace LoanComparer.Api.Controllers
{
    [ApiController]
    [Route("api/offers/{offerId:guid}")]
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
        [Route("document")]
        public async Task<ActionResult> GetContractAsync(Guid offerId)
        {
            var checkResult = await _query.CheckOwnerAsync(offerId, GetUsername());
            if (checkResult == OwnershipTestResult.DoesNotExist)
                return BadRequest();
            if (checkResult == OwnershipTestResult.Unauthorized)
                return Unauthorized();

            var sentInquiryStatus = await _query.GetStatusWithOfferOrThrowAsync(offerId);
            var bank = GetBankInterfaceOrThrow(sentInquiryStatus.BankName);
            Stream fileStream;
            try
            {
                fileStream = await bank.GetDocumentContentAsync(sentInquiryStatus);
            }
            catch (InquiryErrorException e)
            {
                await _command.UpdateStatusAsync(offerId, InquiryStatus.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            var fileName = "contract.txt";
            SetContentDispositionHeader(fileName);
            return File(fileStream, "text/plain", fileName);
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
        [Route("apply")]
        public async Task<ActionResult<ApplyForAnOfferResponse>> ApplyForAnOfferAsync(Guid offerId, IFormFile formFile)
        {
            var checkResult = await _query.CheckOwnerAsync(offerId, GetUsername());
            if (checkResult == OwnershipTestResult.DoesNotExist)
                return BadRequest();
            if (checkResult == OwnershipTestResult.Unauthorized)
                return Unauthorized();
            var sentInquiryStatus = await _query.GetStatusWithOfferOrThrowAsync(offerId);
            if (sentInquiryStatus.Status != InquiryStatus.OfferReceived)
                return BadRequest($"Tried to apply for an offer which status was {sentInquiryStatus.Status}."
                    + $"You can only apply for offers with status {InquiryStatus.OfferReceived}.");
            var bank = GetBankInterfaceOrThrow(sentInquiryStatus.BankName);
            var newInquiryStatus = await bank.ApplyForAnOfferAsync(sentInquiryStatus, formFile);
            if (newInquiryStatus == InquiryStatus.WaitingForAcceptance)
                await _command.UpdateStatusAndApplicationRelatedFieldsAsync(offerId, newInquiryStatus, formFile);
            else
                await _command.UpdateStatusAsync(offerId, newInquiryStatus);
            return ApplyForAnOfferResponse.FromInquiryStatus(newInquiryStatus);
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
