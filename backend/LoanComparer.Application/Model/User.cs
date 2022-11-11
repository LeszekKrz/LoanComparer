using LoanComparer.Application.DTO;
using Microsoft.AspNetCore.Identity;

namespace LoanComparer.Application.Model
{
    public class User : IdentityUser
    {
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public JobType? JobType { get; private set; }
        public int? IncomeLevel { get; private set; }
        public GovernmentId GovernmentId { get; private set; }

        public User(string firstName, string lastName, string email, JobType jobType, int incomeLevel, GovernmentId governmentId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            JobType = jobType;
            IncomeLevel = incomeLevel;
            GovernmentId = governmentId;
            UserName = email; // hack
        }

        private User() { }
    }
}
