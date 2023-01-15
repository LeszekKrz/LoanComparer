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
        public GovernmentIdEntity GovernmentIdEntity { get; private set; }
        public virtual ICollection<UserRole> UserRoles { get; private set; }

        public User(string firstName, string lastName, string email, JobType jobType, int incomeLevel, GovernmentIdEntity governmentIdEntity)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            JobType = jobType;
            IncomeLevel = incomeLevel;
            GovernmentIdEntity = governmentIdEntity;
            UserName = email; // hack
        }

        private User() { }
    }
}
