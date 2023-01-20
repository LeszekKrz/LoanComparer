using LoanComparer.Application.Services.Inquiries.BankInterfaces.Mini;
using LoanComparer.Application.Services.Inquiries.BankInterfaces.This;
using LoanComparer.Application.Services.Offers;
using Microsoft.Extensions.Options;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces;

public sealed class BankInterfaceFactory : IBankInterfaceFactory
{
    private readonly IInquiryCommand _inquiryCommand;
    private readonly IOfferCommand _offerCommand;
    private readonly IOptionsSnapshot<MiniBankConfiguration> _miniConfig;
    private readonly IOptionsSnapshot<ThisBankConfiguration> _thisConfig;
    private MiniBankInterface? _miniBankInterface;
    private ThisBankInterface? _thisBankInterface;

    public BankInterfaceFactory(IInquiryCommand inquiryCommand, IOfferCommand offerCommand,
        IOptionsSnapshot<MiniBankConfiguration> miniConfig, IOptionsSnapshot<ThisBankConfiguration> thisConfig)
    {
        _inquiryCommand = inquiryCommand;
        _offerCommand = offerCommand;
        _miniConfig = miniConfig;
        _thisConfig = thisConfig;
    }
    
    public IReadOnlyList<IBankInterface> CreateBankInterfaces()
    {
        _miniBankInterface ??= new MiniBankInterface(_inquiryCommand, _offerCommand, _miniConfig);
        _thisBankInterface ??= new ThisBankInterface(_inquiryCommand, _offerCommand, _thisConfig);
        return new IBankInterface[] { _miniBankInterface, _thisBankInterface };
    }
}