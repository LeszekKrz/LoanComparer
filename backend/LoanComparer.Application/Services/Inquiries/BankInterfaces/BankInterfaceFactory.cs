using LoanComparer.Application.Services.Inquiries.BankInterfaces.Mini;
using LoanComparer.Application.Services.Offers;
using Microsoft.Extensions.Options;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces;

public sealed class BankInterfaceFactory : IBankInterfaceFactory
{
    private readonly IInquiryCommand _inquiryCommand;
    private readonly IOfferCommand _offerCommand;
    private readonly IOptionsSnapshot<MiniBankConfiguration> _config;
    private MiniBankInterface? _miniBankInterface;

    public BankInterfaceFactory(IInquiryCommand inquiryCommand, IOfferCommand offerCommand, IOptionsSnapshot<MiniBankConfiguration> config)
    {
        _inquiryCommand = inquiryCommand;
        _offerCommand = offerCommand;
        _config = config;
    }
    
    public IReadOnlyList<IBankInterface> CreateBankInterfaces()
    {
        _miniBankInterface ??= new MiniBankInterface(_inquiryCommand, _offerCommand, _config);
        return new[] { _miniBankInterface };
    }
}