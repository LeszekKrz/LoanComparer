using LoanComparer.Application.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole()
                { 
                    Name = LoanComparerConstants.ClientRoleName,
                    NormalizedName= LoanComparerConstants.ClientRoleName.ToUpper()
                },
                new IdentityRole()
                { 
                    Name = LoanComparerConstants.BankEmployeeRoleName,
                    NormalizedName= LoanComparerConstants.BankEmployeeRoleName.ToUpper()
                });

        }
    }
}
