using Flurl.Http;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class MiniBankInterface : BankInterfaceBase
{
    private const string AuthUrl = "https://indentitymanager.snet.com.pl/connect/token";
    private const string BaseUrl = "https://mini.loanbank.api.snet.com.pl/api/v1";

    private ClientWithToken? _clientWithToken;
    
    public override string BankName => "MiNI Bank";
    
    public MiniBankInterface(IInquiryCommand inquiryCommand, IOfferCommand offerCommand) : base(inquiryCommand, offerCommand)
    {
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

        var request = await ConvertInquiryToRequestAsync(inquiry);
        var response = await (
            await _clientWithToken!.Client.
                Request("Inquire").
                PostJsonAsync(request)
            ).
            GetJsonAsync<InquiryCreatedResponse>();

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
        var matchedGovernmentDocumentType = jobTypes.FirstOrDefault(t => t.Name.Equals(inquiry.GovernmentId.Type switch
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

        _clientWithToken = new(new FlurlClient(BaseUrl).WithOAuthBearerToken(token.Value), token);
        return true;
    }
    
    private static async Task<BearerToken?> GetTokenAsync()
    {
        var authClient = new FlurlClient(AuthUrl).AllowAnyHttpStatus()
            .WithBasicAuth("team2b", "EAF5C5C1-ADF4-4F35-AE02-44C61B0CE842");
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
            Percentage = response.Percentage
        };

        // TODO: Save document link (and valid date) somewhere
        return new()
        {
            Id = status.Id,
            BankName = status.BankName,
            Inquiry = status.Inquiry,
            ReceivedOffer = offer,
            Status = InquiryStatus.Accepted
        };
    }

    public override Task<FileResult> GetDocumentAsync(Guid offerId)
    {
        throw new NotImplementedException();
    }

    public override Task<SentInquiryStatus> ApplyForAnOfferAsync(Guid offerId, IFormFile file)
    {
        throw new NotImplementedException();
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

        public static BearerToken FromResponse(AuthenticationResponse response, DateTime requestTime)
        {
            return new(response.AccessToken, requestTime + TimeSpan.FromSeconds(response.ExpiresIn));
        }
    }

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
        public string AccessToken { get; init; } = null!;
        public int ExpiresIn { get; init; }
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
        public DateTime DocumentLinkValidDate { get; init; }
    }
}