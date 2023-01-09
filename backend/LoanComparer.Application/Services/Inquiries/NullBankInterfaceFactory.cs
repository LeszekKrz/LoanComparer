namespace LoanComparer.Application.Services.Inquiries;

/// <summary>
///     This class was created so that migrations can be made
/// </summary>
public sealed class NullBankInterfaceFactory : IBankInterfaceFactory
{
    public IReadOnlyList<IBankInterface> CreateBankInterfaces()
    {
        return Array.Empty<IBankInterface>();
    }
}