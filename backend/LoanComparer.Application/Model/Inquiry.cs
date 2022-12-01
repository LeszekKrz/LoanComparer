namespace LoanComparer.Application.Model
{
    public class Inquiry
    {
        public int Id { get; init; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string GovernmentIdType { get; private set; }
        public string GovernmentIdValue { get; private set; }
        public JobType JobType { get; private set; }
        public int IncomeLevel { get; private set; }
        public int AmountOfMoneyToLoan { get; private set; }
        public int NumberOfInstallments { get; private set; }
        public DateTime SubmitDate { get; private set; } = DateTime.UtcNow;

        public Inquiry(string firstName, string lastName, string governmentIdType, string governmentIdValue, JobType jobType, int incomeLevel,            int amountOfMoneyToLoan, int numberOfInstallments)        {
            FirstName = firstName;
            LastName = lastName;
            GovernmentIdType = governmentIdType;
            GovernmentIdValue = governmentIdValue;
            JobType = jobType;
            IncomeLevel = incomeLevel;
            AmountOfMoneyToLoan = amountOfMoneyToLoan;
            NumberOfInstallments = numberOfInstallments;
        }

        public Inquiry() { }
    }
}
