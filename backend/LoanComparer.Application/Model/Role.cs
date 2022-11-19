using Microsoft.AspNetCore.Identity;

namespace LoanComparer.Application.Model
{
    public class Role : IdentityRole
    {
        public virtual ICollection<UserRole> UsersRoles { get; private set; }

        internal Role() { }
    }
}
