using Microsoft.AspNetCore.Identity;

namespace LoanComparer.Application.Model
{
    public class User : IdentityUser
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public JobType? JobType { get; init; }
        public int? IncomeLevel { get; init; }
        public GovernmentIdEntity? GovernmentIdEntity { get; init; }
        public virtual ICollection<UserRole> UserRoles { get; init; }

        public User(string firstName, string lastName, string email, JobType jobType, int incomeLevel, GovernmentIdEntity governmentIdEntity)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            JobType = jobType;
            IncomeLevel = incomeLevel;
            GovernmentIdEntity = governmentIdEntity;
            UserName = email;
        }

        public User() { }
    }
}
