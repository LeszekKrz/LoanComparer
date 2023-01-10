namespace LoanComparer.Application.Services.Inquiries;

/// <summary>
///     This class was created so that migrations can be made
/// </summary>
public sealed class MockBankInterfaceFactory : IBankInterfaceFactory
{
    public IReadOnlyList<IBankInterface> CreateBankInterfaces()
    {
        return new IBankInterface[] { new RejectingBankInterface(), new AcceptingBankInterface() };
    }
}