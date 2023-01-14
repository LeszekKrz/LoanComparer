using LoanComparer.Application.Services.Offers;

namespace LoanComparer.Application.Services.Inquiries.BankInterfaces.Mock;

/// <summary>
///     This class was created so that migrations can be made
/// </summary>
public sealed class MockBankInterfaceFactory : IBankInterfaceFactory
{
    private readonly IInquiryCommand _inquiryCommand;
    private readonly IOfferCommand _offerCommand;

    public MockBankInterfaceFactory(IInquiryCommand inquiryCommand, IOfferCommand offerCommand)
    {
        _inquiryCommand = inquiryCommand;
        _offerCommand = offerCommand;
    }
    
    public IReadOnlyList<IBankInterface> CreateBankInterfaces()
    {
        return new IBankInterface[]
        {
            new RejectingBankInterface(_inquiryCommand, _offerCommand),
            new AcceptingBankInterface(_inquiryCommand, _offerCommand)
        };
    }
}