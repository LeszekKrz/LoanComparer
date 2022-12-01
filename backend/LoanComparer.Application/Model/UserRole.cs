using Microsoft.AspNetCore.Identity;

namespace LoanComparer.Application.Model
{
    public class UserRole : IdentityUserRole<string>
    {
        public User User { get; private set; }
        public Role Role { get; private set; }

        public UserRole() { }
    }
}
