using LoanComparer.Application.Constants;
using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration
{
    public class GovernmentIdEntityConfiguration : IEntityTypeConfiguration<GovernmentIdEntity>
    {
        public void Configure(EntityTypeBuilder<GovernmentIdEntity> builder)
        {
            builder.Property(x => x.Type).HasMaxLength(LoanComparerConstants.MaxGovernmentIdTypeLength).HasColumnName("GovernmentIdType");
            builder.Property(x => x.Value).HasMaxLength(LoanComparerConstants.MaxGovernmentIdValueLength).HasColumnName("GovernmentIdValue");

            builder.ToTable("AspNetUsers");
        }
    }
}
