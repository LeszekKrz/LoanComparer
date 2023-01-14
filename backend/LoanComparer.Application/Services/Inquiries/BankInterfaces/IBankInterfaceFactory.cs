namespace LoanComparer.Application.Services.Inquiries.BankInterfaces;

public interface IBankInterfaceFactory
{
    IReadOnlyList<IBankInterface> CreateBankInterfaces();
}