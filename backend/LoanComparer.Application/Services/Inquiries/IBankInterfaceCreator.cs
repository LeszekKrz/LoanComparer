namespace LoanComparer.Application.Services.Inquiries;

public interface IBankInterfaceCreator
{
    IReadOnlyList<IBankInterface> CreateBankInterfaces();
}