using Microsoft.AspNetCore.Identity;

namespace LoanComparer.Application.Model
{
    public class User : IdentityUser
    {
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public JobType? JobType { get; private set; }
        public int? IncomeLevel { get; private set; }
        public string? GovernmentIdType { get; private set; }
        public string? GovernmentIdValue { get; private set; }

        public User(string firstName, string lastName, string email, JobType jobType, int incomeLevel, string governmentIdType,
            string governmentIdValue)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            JobType = jobType;
            IncomeLevel = incomeLevel;
            GovernmentIdType = governmentIdType;
            GovernmentIdValue = governmentIdValue;
            UserName = email; // hack
        }

        private User() { }
    }
}
