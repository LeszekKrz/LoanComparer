using System.Security.Authentication;
using Flurl.Http;
using LoanComparer.Application.Constants;
using LoanComparer.Application.DTO.OfferApplicationDTO;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services.Offers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces.This;

public sealed class ThisBankInterface : BankInterfaceBase
{
    private readonly IOptionsSnapshot<ThisBankConfiguration> _config;
    private ClientWithToken? _clientWithToken;
    private ClientWithToken? _adminClientWithToken;

    public ThisBankInterface(IInquiryCommand inquiryCommand, IOfferCommand offerCommand,
        IOptionsSnapshot<ThisBankConfiguration> config) : base(inquiryCommand, offerCommand)
    {
        _config = config;
    }

    public override string BankName => LoanComparerConstants.OurBankName;
    
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
                await _clientWithToken!.Client.Request("inquire").PostJsonAsync(request)
            ).GetJsonAsync<OfferResponse>();

            return new SentInquiryStatus
            {
                Id = Guid.NewGuid(),
                BankName = BankName,
                Inquiry = inquiry,
                ReceivedOffer = new()
                {
                    Id = response.Id,
                    LoanValue = (decimal)response.Amount,
                    NumberOfInstallments = response.Installments,
                    Percentage = response.Percentage,
                    MonthlyInstallment = (decimal)(response.Amount * (1 + response.Percentage / 100) / response.Installments),
                    DocumentLink = response.Id.ToString()
                },
                Status = InquiryStatus.OfferReceived
            };
        }
        catch (Exception e) when (e is FlurlHttpException or FormatException)
        {
            if (e is FlurlHttpException { StatusCode: StatusCodes.Status422UnprocessableEntity })
            {
                return new()
                {
                    Id = Guid.NewGuid(),
                    BankName = BankName,
                    Inquiry = inquiry,
                    ReceivedOffer = null,
                    Status = InquiryStatus.Rejected
                };
            }
            
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

    protected override Task<SentInquiryStatus> GetRefreshedStatusAsync(SentInquiryStatus status) =>
        Task.FromResult(status);

    private async Task<ThisBankInquiryRequest> ConvertInquiryToRequestAsync(Inquiry inquiry)
    {
        if (_clientWithToken is null) throw new InvalidOperationException("Flurl client is not created");

        var jobTypes = await (
            await _clientWithToken.Client.
                Request("job-types").
                GetAsync()
            ).
            GetJsonAsync<IReadOnlyList<JobTypeResponse>>();
        var matchedJobType = jobTypes.FirstOrDefault(t =>
            t.Name.Equals(inquiry.JobDetails.JobName, StringComparison.OrdinalIgnoreCase));
        var unknownJobId =
            jobTypes.FirstOrDefault(t => t.Name.Equals("unknown", StringComparison.OrdinalIgnoreCase))?.Id ??
            throw new FormatException();
        
        var governmentDocumentTypes = await (
                await _clientWithToken.Client.
                    Request("government-id-types").
                    GetAsync()
            ).
            GetJsonAsync<IReadOnlyList<GovernmentDocumentTypeResponse>>();
        var matchedGovernmentDocumentType = governmentDocumentTypes.FirstOrDefault(t =>
            t.Name == inquiry.GovernmentId.Type switch
            {
                "ID Number" => "Government ID",
                "Passport Number" => "Passport",
                "PESEL" => "Social Security Number",
                var other => other
            });
        var unknownGovIdType = governmentDocumentTypes.FirstOrDefault(t => t.Name == "Other")?.Id ??
                               throw new FormatException();

        return new()
        {
            Amount = (double)inquiry.AmountRequested,
            Installments = inquiry.NumberOfInstallments,
            FirstName = inquiry.PersonalData.FirstName,
            LastName = inquiry.PersonalData.LastName,
            Income = (int)inquiry.JobDetails.IncomeLevel,
            JobType = new()
            {
                Id = matchedJobType?.Id ?? unknownJobId,
                Name = matchedJobType?.Name ?? inquiry.JobDetails.JobName
            },
            GovernmentId = new()
            {
                Id = matchedGovernmentDocumentType?.Id ?? unknownGovIdType,
                Name = matchedGovernmentDocumentType?.Name ?? inquiry.GovernmentId.Type,
                Value = inquiry.GovernmentId.Value
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
        if (!await EnsureUserIsCreatedAsync()) return null;

        var password = GetEnv("THIS_BANK_API_PASSWORD");
        var authClient = new FlurlClient(_config.Value.BaseUrl).AllowAnyHttpStatus();
        var response = await authClient.Request("users", "auth").PostJsonAsync(new
        {
            Username = _config.Value.AuthUsername,
            Password = password
        });

        if (response.StatusCode != StatusCodes.Status200OK) return null;
        var authResponse = await response.GetJsonAsync<AuthenticationResponse>();
        return BearerToken.FromResponse(authResponse);
    }

    private async Task<bool> EnsureUserIsCreatedAsync()
    {
        var authClient = new FlurlClient(_config.Value.BaseUrl).AllowAnyHttpStatus();
        var userExistsResponse = await authClient.Request("users", "exists").
            SetQueryParam("username", _config.Value.AuthUsername).
            GetAsync();
        if (userExistsResponse.StatusCode == StatusCodes.Status204NoContent) return true;

        var registrationResponse = await authClient.Request("users", "register").PostJsonAsync(new
        {
            Username = _config.Value.AuthUsername,
            Password = GetEnv("THIS_BANK_API_PASSWORD"),
            Key = GetEnv("REGISTRATION_KEY")
        });

        return registrationResponse.StatusCode == StatusCodes.Status201Created;
    }

    private async Task<bool> EnsureAdminClientIsValidAsync()
    {
        if (_adminClientWithToken?.Token.IsValid ?? false) return true;

        var token = await GetAdminTokenAsync();
        if (token is null)
        {
            _adminClientWithToken = null;
            return false;
        }

        _adminClientWithToken = new(new FlurlClient(_config.Value.BaseUrl).WithOAuthBearerToken(token.Value), token);
        return true;
    }

    private async Task<BearerToken?> GetAdminTokenAsync()
    {
        var adminUsernameAndPassword = GetEnv("SEED_ADMIN_CREDENTIALS").Split(':');

        var authClient = new FlurlClient(_config.Value.BaseUrl).AllowAnyHttpStatus();
        var response = await authClient.Request("users", "auth").PostJsonAsync(new
        {
            Username = adminUsernameAndPassword[0],
            Password = adminUsernameAndPassword[1]
        });

        if (response.StatusCode != StatusCodes.Status200OK) return null;
        var authResponse = await response.GetJsonAsync<AuthenticationResponse>();
        return BearerToken.FromResponse(authResponse);
    }

    public override async Task<Stream> GetDocumentContentAsync(SentInquiryStatus sentInquiryStatus)
    {
        if (sentInquiryStatus.ReceivedOffer is null)
            throw new InvalidOperationException(
                "Cannot retrieve a document for this inquiry because no offer was received");
        
        if (!await EnsureClientIsValidAsync())
            throw new InvalidCredentialException("Authorization to Bank API failed");

        return await _clientWithToken!
            .Client
            .Request("offers", sentInquiryStatus.ReceivedOffer.DocumentLink, "document")
            .GetStreamAsync();
    }

    public override async Task<InquiryStatus> ApplyForAnOfferAsync(SentInquiryStatus sentInquiryStatus, IFormFile file)
    {
        if (sentInquiryStatus.ReceivedOffer is null)
            throw new InvalidOperationException(
                "Cannot apply for an offer related to this inquiry because no offer was received");
        
        if (!await EnsureClientIsValidAsync())
            return InquiryStatus.Error;

        await using var stream = file.OpenReadStream();
        await _clientWithToken!.Client.
            Request("apply", sentInquiryStatus.ReceivedOffer.Id).
            PostMultipartAsync(mp =>
            {
                mp.AddFile("file", stream, file.FileName, file.ContentType);
            });

        return InquiryStatus.WaitingForAcceptance;
    }

    public async Task<InquiryStatus> ReviewApplicationAsync(SentInquiryStatus sentInquiryStatus, ReviewApplicationRequestDTO reviewApplicationRequest)
    {
        if (sentInquiryStatus.ReceivedOffer is null)
            throw new InvalidOperationException(
                "Cannot review for an offer related to this inquiry because no offer was received");

        if (!await EnsureAdminClientIsValidAsync())
            return InquiryStatus.Error;

        ReviewRequest reviewRequest = new()
        {
            Accept = reviewApplicationRequest.Accept
        };

        var response = await _adminClientWithToken!
                    .Client
                    .Request("applications", sentInquiryStatus.ReceivedOffer.Id, "review")
                    .PostJsonAsync(reviewRequest);

        return reviewApplicationRequest.Accept ? InquiryStatus.Accepted : InquiryStatus.Rejected;
    }

    private static string GetEnv(string name)
    {
        return Environment.GetEnvironmentVariable(name) ??
               throw new InvalidOperationException($"Environment variable {name} is not defined");
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

        public static BearerToken? FromResponse(AuthenticationResponse response)
        {
            if (response.Token is null || response.ExpirationTime is null) return null;
            return new(response.Token, response.ExpirationTime.Value);
        }
    }

    #region Dto classes
    private sealed class JobTypeResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }

    private sealed class GovernmentDocumentTypeResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
    }
    
    private sealed class AuthenticationResponse
    {
        public string? Token { get; init; }

        public DateTime? ExpirationTime { get; init; }
    }

    private sealed class ThisBankInquiryRequest
    {
        public double Amount { get; init; }
        public int Installments { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public int Income { get; init; }
        public GovernmentDocumentRequest GovernmentId { get; init; } = null!;
        public JobTypeRequest JobType { get; init; } = null!;

        public sealed class GovernmentDocumentRequest
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string Value { get; init; }
        }

        public sealed class JobTypeRequest
        {
            public int Id { get; init; }
            public string Name { get; init; }
        }
    }

    private sealed class OfferResponse
    {
        public Guid Id { get; init; }
        public DateTime CreationDate { get; init; }
        public double Amount { get; init; }
        public int Installments { get; init; }
        public double Percentage { get; init; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public int Income { get; init; }
        public GovernmentIdResponse GovernmentId { get; init; } = null!;
        public JobTypeResponse JobType { get; init; } = null!;

        public sealed class GovernmentIdResponse
        {
            public int Id { get; init; }
            public string Name { get; init; } = null!;
            public string Value { get; init; } = null!;
        }
    }

    private sealed class ReviewRequest
    {
        public bool Accept { get; init; }
    }
    #endregion
}