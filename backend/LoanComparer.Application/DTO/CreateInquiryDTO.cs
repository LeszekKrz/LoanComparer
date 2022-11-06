namespace LoanComparer.Application.DTO
{
    public record CreateInquiryDTO(string FirstName, string LastName, string GovernmentIdType, string GovernmentIdValue, JobTypeDTO JobType, int IncomeLevel
        int AmountOfMoneyToLoan, int NumberOfInstallments);
}
