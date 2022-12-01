using Microsoft.AspNetCore.Identity;

namespace LoanComparer.Application.Model
{
    public class Role : IdentityRole
    {
        public virtual ICollection<UserRole> UsersRoles { get; private set; }

        internal Role(string roleName) : base(roleName)
        {
            NormalizedName = roleName.ToUpper();
        }
        private Role() { }
    }
}
