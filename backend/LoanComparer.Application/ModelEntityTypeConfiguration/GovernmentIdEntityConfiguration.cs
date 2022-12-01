using LoanComparer.Application.Constants;
using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration
{
    public class GovernmentIdEntityConfiguration : IEntityTypeConfiguration<GovernmentId>
    {
        public void Configure(EntityTypeBuilder<GovernmentId> builder)
        {
            builder.Property(x => x.Type).HasMaxLength(LoanComparerConstants.MaxGovernmentIdTypeLength).HasColumnName("GovernmentIdType");
            builder.Property(x => x.Value).HasMaxLength(LoanComparerConstants.MaxGovernmentIdValueLength).HasColumnName("GovernmentIdValue");

            builder.ToTable("AspNetUsers");
        }
    }
}
