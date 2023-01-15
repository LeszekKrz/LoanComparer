using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration;

public sealed class InquiryEntityConfiguration : IEntityTypeConfiguration<InquiryEntity>
{
    public void Configure(EntityTypeBuilder<InquiryEntity> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).IsRequired();
        builder.Property(i => i.NotificationEmail).IsRequired();
        builder.Property(i => i.CreationTimestamp).IsRequired();
        builder.Property(i => i.AmountRequestedAsSmallestNominal).IsRequired();
        builder.Property(i => i.FirstName).IsRequired();
        builder.Property(i => i.LastName).IsRequired();
        builder.Property(i => i.JobName).IsRequired();
        builder.Property(i => i.IncomeLevelAsSmallestNominal).IsRequired();
        builder.Property(i => i.GovernmentIdType).IsRequired();
        builder.Property(i => i.GovernmentIdValue).IsRequired();
    }
}