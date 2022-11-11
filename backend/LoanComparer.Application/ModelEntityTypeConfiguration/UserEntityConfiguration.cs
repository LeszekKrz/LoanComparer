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
            // I guess we can ignore phone number related things...

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.JobType)
                .WithMany(x => x.Users)
                .HasForeignKey("JobTypeName");

            builder.HasOne(x => x.GovernmentId)
                .WithOne(x => x.User)
                .HasForeignKey<GovernmentId>(x => x.Id);

            builder.Property(x => x.FirstName).HasMaxLength(LoanComparerConstants.MaxFirstNameLength);
            builder.Property(x => x.LastName).HasMaxLength(LoanComparerConstants.MaxLastNameLength);
        }
    }
}
