using LoanComparer.Application.Constants;
using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration
{
    public class JobTypeEntityConfiguration : IEntityTypeConfiguration<JobType>
    {
        public void Configure(EntityTypeBuilder<JobType> builder)
        {
            builder.HasKey(x => x.Name);
            builder.Property(x => x.Name).HasMaxLength(LoanComparerConstants.MaxJobTypeNameLength).IsRequired();
        }
    }
}
