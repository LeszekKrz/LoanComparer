﻿using LoanComparer.Application.Constants;
using LoanComparer.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanComparer.Application.ModelEntityTypeConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.JobType)
                .WithMany(x => x.Users)
                .HasForeignKey("JobTypeName");

            builder.HasOne(x => x.GovernmentIdEntity)
                .WithOne(x => x.User)
                .HasForeignKey<GovernmentIdEntity>(x => x.Id);

            builder.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(userRole => userRole.UserId)
                .IsRequired();

            builder.Property(x => x.FirstName).HasMaxLength(LoanComparerConstants.MaxFirstNameLength);
            builder.Property(x => x.LastName).HasMaxLength(LoanComparerConstants.MaxLastNameLength);
        }
    }
}
