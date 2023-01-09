namespace LoanComparer.Application.Services.Inquiries;

public interface IBankInterfaceFactory
{
    IReadOnlyList<IBankInterface> CreateBankInterfaces();
}