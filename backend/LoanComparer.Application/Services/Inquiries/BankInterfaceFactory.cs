using LoanComparer.Application.Services.Offers;

namespace LoanComparer.Application.Services.Inquiries;

public sealed class BankInterfaceFactory : IBankInterfaceFactory
{
    private readonly IInquiryCommand _inquiryCommand;
    private readonly IOfferCommand _offerCommand;
    private MiniBankInterface? _miniBankInterface;

    public BankInterfaceFactory(IInquiryCommand inquiryCommand, IOfferCommand offerCommand)
    {
        _inquiryCommand = inquiryCommand;
        _offerCommand = offerCommand;
    }
    
    public IReadOnlyList<IBankInterface> CreateBankInterfaces()
    {
        _miniBankInterface ??= new MiniBankInterface(_inquiryCommand, _offerCommand);
        return new[] { _miniBankInterface };
    }
}