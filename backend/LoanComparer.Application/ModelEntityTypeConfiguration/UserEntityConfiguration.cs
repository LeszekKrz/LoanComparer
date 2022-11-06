using LoanComparer.Application.Constants;
using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //builder.Ignore(x => x.UserName);
            //builder.Ignore(x => x.NormalizedUserName);
            //builder.Ignore(x => x.PhoneNumber);
            //builder.Ignore(x => x.PhoneNumberConfirmed);
            //builder.Ignore(x => x.TwoFactorEnabled);

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.JobType)
                .WithMany(x => x.Users)
                .HasForeignKey("JobTypeName");

            builder.Property(x => x.FirstName).HasMaxLength(LoanComparerConstants.MaxFirstNameLength);
            builder.Property(x => x.LastName).HasMaxLength(LoanComparerConstants.MaxLastNameLength);
            builder.Property(x => x.GovernmentIdType).HasMaxLength(LoanComparerConstants.MaxGovernmentIdTypeLength);
            builder.Property(x => x.GovernmentIdValue).HasMaxLength(LoanComparerConstants.MaxGovernmentIdValueLength);
        }
    }
}
