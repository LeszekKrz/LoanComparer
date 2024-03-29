﻿using System.Security.Authentication;
using Flurl.Http;
using LoanComparer.Application.Exceptions;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces.Mini;

public sealed class MiniBankInterface : BankInterfaceBase
{
    private readonly IOptionsSnapshot<MiniBankConfiguration> _config;
    private ClientWithToken? _clientWithToken;
    
    public override string BankName => "MiNI Bank";

    public MiniBankInterface(IInquiryCommand inquiryCommand, IOfferCommand offerCommand,
        IOptionsSnapshot<MiniBankConfiguration> config) : base(inquiryCommand, offerCommand)
    {
        _config = config;
    }

    public override async Task<SentInquiryStatus> SendInquiryAsync(Inquiry inquiry)
    {
        if (!await EnsureClientIsValidAsync())
            return new()
            {
                Id = Guid.NewGuid(),
                BankName = BankName,
                Inquiry = inquiry,
                ReceivedOffer = null,
                Status = InquiryStatus.Error
            };

        try
        {
            var request = await ConvertInquiryToRequestAsync(inquiry);
            var response = await (
                await _clientWithToken!.Client.Request("Inquire").PostJsonAsync(request)
            ).GetJsonAsync<InquiryCreatedResponse>();

            return new SentInquiryStatus
            {
                Id = Guid.NewGuid(),
                BankName = BankName,
                Inquiry = inquiry,
                ReceivedOffer = null,
                Status = InquiryStatus.Pending,
                AdditionalData = new AdditionalStatusData
                {
                    InquireId = response.InquireId,
                    OfferId = null
                }.Serialize()
            };
        }
        catch (FlurlHttpException)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                BankName = BankName,
                Inquiry = inquiry,
                ReceivedOffer = null,
                Status = InquiryStatus.Error
            };
        }
    }
    
    protected override async Task<SentInquiryStatus> GetRefreshedStatusAsync(SentInquiryStatus status)
    {
        if (!await EnsureClientIsValidAsync())
            return new()
            {
                Id = status.Id,
                BankName = status.BankName,
                Inquiry = status.Inquiry,
                ReceivedOffer = null,
                Status = InquiryStatus.Error
            };

        var additionalData = AdditionalStatusData.Deserialize(status.AdditionalData);
        var response = await (
                await _clientWithToken!.Client.
                    Request("Inquire", additionalData.InquireId).
                    GetAsync()
            ).
            GetJsonAsync<InquiryStatusResponse>();

        return response.StatusId switch
        {
            1 => new()
            {
                Id = status.Id,
                BankName = status.BankName,
                Inquiry = status.Inquiry,
                ReceivedOffer = null,
                Status = InquiryStatus.Pending
            },
            2 => new()
            {
                Id = status.Id,
                BankName = status.BankName,
                Inquiry = status.Inquiry,
                ReceivedOffer = null,
                Status = InquiryStatus.Rejected
            },
            3 => await GetUpdatedStatusWithOfferAsync(status,
                response.OfferId ??
                throw new InvalidOperationException(
                    "Received status indicating that offer was created, yet offer ID is null")),
            _ => new()
            {
                Id = status.Id,
                BankName = status.BankName,
                Inquiry = status.Inquiry,
                ReceivedOffer = null,
                Status = InquiryStatus.Error
            }
        };
    }

    private async Task<MiniBankInquiryRequest> ConvertInquiryToRequestAsync(Inquiry inquiry)
    {
        if (_clientWithToken is null) throw new InvalidOperationException();

        var jobTypes = await (
            await _clientWithToken.Client.
                Request("Dictionary", "jobTypes").
                GetAsync()
            ).
            GetJsonAsync<IReadOnlyList<JobTypeResponse>>();
        var matchedJobType = jobTypes.FirstOrDefault(t =>
            t.Name.Equals(inquiry.JobDetails.JobName, StringComparison.OrdinalIgnoreCase));
        
        var governmentDocumentTypes = await (
                await _clientWithToken.Client.
                    Request("Dictionary", "governmentDocumentTypes").
                    GetAsync()
            ).
            GetJsonAsync<IReadOnlyList<GovernmentDocumentTypeResponse>>();
        var matchedGovernmentDocumentType = governmentDocumentTypes.FirstOrDefault(t =>
            t.Name.Equals(inquiry.GovernmentId.Type switch
            {
                "ID Number" => "Government Id",
                "Passport Number" => "Passport",
                _ => "Other"
            }, StringComparison.OrdinalIgnoreCase));

        return new()
        {
            Value = (double)inquiry.AmountRequested,
            InstallmentsNumber = inquiry.NumberOfInstallments,
            JobDetails = new()
            {
                TypeId = matchedJobType?.Id,
                Name = matchedJobType?.Name ?? inquiry.JobDetails.JobName,
                Description = inquiry.JobDetails.Description,
                JobStartDate = inquiry.JobDetails.StartDate?.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero)),
                JobEndDate = inquiry.JobDetails.EndDate?.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero))
            },
            PersonalData = new()
            {
                FirstName = inquiry.PersonalData.FirstName,
                LastName = inquiry.PersonalData.LastName,
                BirthDate = inquiry.PersonalData.BirthDate?.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero))
            },
            GovernmentDocument = new()
            {
                TypeId = matchedGovernmentDocumentType?.Id,
                Name = matchedGovernmentDocumentType?.Name ?? inquiry.GovernmentId.Type,
                Description = matchedGovernmentDocumentType?.Description,
                Number = inquiry.GovernmentId.Value
            }
        };
    }
    
    private async Task<bool> EnsureClientIsValidAsync()
    {
        if (_clientWithToken?.Token.IsValid ?? false) return true;

        var token = await GetTokenAsync();
        if (token is null)
        {
            _clientWithToken = null;
            return false;
        }

        _clientWithToken = new(new FlurlClient(_config.Value.BaseUrl).WithOAuthBearerToken(token.Value), token);
        return true;
    }
    
    private async Task<BearerToken?> GetTokenAsync()
    {
        var password = Environment.GetEnvironmentVariable("MINI_BANK_API_PASSWORD") ??
                       throw new InvalidCredentialException(
                           "Environment variable MINI_BANK_API_PASSWORD is not defined");
        var authClient = new FlurlClient(_config.Value.AuthUrl).
            AllowAnyHttpStatus().
            WithBasicAuth(_config.Value.AuthUsername, password);
        var requestTime = DateTime.Now;
        var response = await authClient.Request().PostUrlEncodedAsync(new
        {
            grant_type = "client_credentials",
            scope = "MiNI.LoanBank.API"
        });

        if (response.StatusCode != StatusCodes.Status200OK) return null;
        var authResponse = await response.GetJsonAsync<AuthenticationResponse>();
        return BearerToken.FromResponse(authResponse, requestTime);
    }

    private async Task<SentInquiryStatus> GetUpdatedStatusWithOfferAsync(SentInquiryStatus status, int offerId)
    {
        var response = await (
                await _clientWithToken!.Client.
                    Request("Offer", offerId).
                    GetAsync()
            ).
            GetJsonAsync<OfferResponse>();

        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            LoanValue = (decimal)response.RequestedValue,
            MonthlyInstallment = (decimal)response.MonthlyInstallment,
            NumberOfInstallments = response.RequestedPeriodInMonth,
            Percentage = response.Percentage,
            DocumentLink = response.DocumentLink,
        };

        var newInquiryStatus = response.StatusId switch
        {
            1 => InquiryStatus.OfferReceived,
            2 => InquiryStatus.Rejected,
            3 => InquiryStatus.OfferReceived,
            4 => InquiryStatus.WaitingForAcceptance,
            5 => InquiryStatus.Accepted,
            _ => InquiryStatus.Error,
        };

        return new()
        {
            Id = status.Id,
            BankName = status.BankName,
            Inquiry = status.Inquiry,
            ReceivedOffer = offer,
            Status = newInquiryStatus,
            AdditionalData = new AdditionalStatusData
            {
                InquireId = response.InquireId,
                OfferId = offerId,
            }.Serialize()
        };
    }
    
    public override async Task<Stream> GetDocumentContentAsync(SentInquiryStatus sentInquiryStatus)
    {
        if (!await EnsureClientIsValidAsync())
            throw new InvalidCredentialException("An error has occured while trying to get mini bank interface credentials");

        var additionalData = AdditionalStatusData.Deserialize(sentInquiryStatus.AdditionalData);
        if (additionalData.OfferId is null)
            throw new InquiryErrorException("Error trying to get the document from mini bank because offerid was null."
                + $"Related offerid in our system: {sentInquiryStatus.ReceivedOffer!.Id}");

        return await _clientWithToken!
            .Client
            .Request("Offer", additionalData.OfferId, "document", sentInquiryStatus.ReceivedOffer!.DocumentLink.Split('/')[^1])
            .GetStreamAsync();
    }

    public override async Task<InquiryStatus> ApplyForAnOfferAsync(SentInquiryStatus sentInquiryStatus, IFormFile file)
    {
        if (!await EnsureClientIsValidAsync())
            throw new InvalidCredentialException("An error has occured while trying to get mini bank interface credentials");

        var additionalData = AdditionalStatusData.Deserialize(sentInquiryStatus.AdditionalData);
        if (additionalData.OfferId is null)
            return InquiryStatus.Error;

        await using var stream = file.OpenReadStream();
        await _clientWithToken!
            .Client
            .Request("Offer", additionalData.OfferId, "document", "upload")
            .PostMultipartAsync(mp =>
            {
                mp.AddFile("formFile", stream, file.FileName, file.ContentType);
            });

        return InquiryStatus.WaitingForAcceptance;
    }

    private sealed record ClientWithToken(IFlurlClient Client, BearerToken Token); 
    
    private sealed record BearerToken(string Value)
    {
        private readonly DateTime _expirationTime;

        public bool IsValid => DateTime.Now + TimeSpan.FromSeconds(5) < _expirationTime;

        private BearerToken(string value, DateTime expirationTime) : this(value)
        {
            _expirationTime = expirationTime;
        }

        public static BearerToken? FromResponse(AuthenticationResponse response, DateTime requestTime)
        {
            if (response.AccessToken is null || response.ExpiresIn <= 0) return null;
            return new(response.AccessToken, requestTime + TimeSpan.FromSeconds(response.ExpiresIn));
        }
    }
    
    private sealed record AdditionalStatusData
    {
        public int InquireId { get; init; }
        public int? OfferId { get; init; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AdditionalStatusData Deserialize(string? serialized)
        {
            if (serialized is null) throw new ArgumentNullException(nameof(serialized));
            return JsonConvert.DeserializeObject<AdditionalStatusData>(serialized) ??
                   throw new InvalidOperationException($"Additional status data is in invalid format: [{serialized}]");
        }
    }
    
    #region Dto classes
    private sealed class JobTypeResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
    }

    private sealed class GovernmentDocumentTypeResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
    }
    
    private sealed class AuthenticationResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; init; }
        
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; init; }
        
        [JsonProperty("token_type")]
        public string TokenType { get; init; } = null!;
        
        public string Scope { get; init; } = null!;
    }

    private sealed class MiniBankInquiryRequest
    {
        public double Value { get; init; }
        public int InstallmentsNumber { get; init; }
        public PersonalDataRequest PersonalData { get; init; } = null!;
        public GovernmentDocumentRequest GovernmentDocument { get; init; } = null!;
        public JobDetailsRequest JobDetails { get; init; } = null!;

        public sealed class PersonalDataRequest
        {
            public string? FirstName { get; init; }
            public string? LastName { get; init; }
            public DateTime? BirthDate { get; init; }
        }

        public sealed class GovernmentDocumentRequest
        {
            public int? TypeId { get; init; }
            public string? Name { get; init; }
            public string? Description { get; init; }
            public string? Number { get; init; }
        }

        public sealed class JobDetailsRequest
        {
            public int? TypeId { get; init; }
            public string? Name { get; init; }
            public string? Description { get; init; }
            public DateTime? JobStartDate { get; init; }
            public DateTime? JobEndDate { get; init; }
        }
    }

    private sealed class InquiryCreatedResponse
    {
        public int InquireId { get; init; }
        public DateTime CreateDate { get; init; }
    }

    private sealed class InquiryStatusResponse
    {
        public int InquireId { get; init; }
        public DateTime CreateDate { get; init; }
        public int StatusId { get; init; }
        public string StatusDescription { get; init; } = null!;
        public int? OfferId { get; init; }
    }

    private sealed class OfferResponse
    {
        public int Id { get; init; }
        public double Percentage { get; init; }
        public double MonthlyInstallment { get; init; }
        public double RequestedValue { get; init; }
        public int RequestedPeriodInMonth { get; init; }
        public int StatusId { get; init; }
        public string StatusDescription { get; init; } = null!;
        public int InquireId { get; init; }
        public DateTime CreateDate { get; init; }
        public DateTime UpdateDate { get; init; }
        public string? ApprovedBy { get; init; }
        public string DocumentLink { get; init; } = null!;
        public DateTime? DocumentLinkValidDate { get; init; }
    }
    #endregion
}