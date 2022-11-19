using LoanComparer.Application.Constants;
using LoanComparer.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasMany(x => x.UsersRoles)
                .WithOne(x => x.Role)
                .HasForeignKey(userRole => userRole.RoleId)
                .IsRequired();

            builder.HasData(
                new Role()
                { 
                    Name = LoanComparerConstants.ClientRoleName,
                    NormalizedName= LoanComparerConstants.ClientRoleName.ToUpper()
                },
                new Role()
                { 
                    Name = LoanComparerConstants.BankEmployeeRoleName,
                    NormalizedName= LoanComparerConstants.BankEmployeeRoleName.ToUpper()
                });

        }
    }
}
