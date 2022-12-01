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

            builder.HasData(
                new JobType("Director"),
                new JobType("Agent"),
                new JobType("Administrator"),
                new JobType("Coordinator"),
                new JobType("Specialist"),
                new JobType("Orchestrator"),
                new JobType("Assistant"),
                new JobType("Designer"),
                new JobType("Facilitator"),
                new JobType("Analyst"),
                new JobType("Producer"),
                new JobType("Technician"),
                new JobType("Manager"),
                new JobType("Liaison"),
                new JobType("Associate"),
                new JobType("Consultant"),
                new JobType("Engineer"),
                new JobType("Strategist"),
                new JobType("Supervisor"),
                new JobType("Executive"),
                new JobType("Planner"),
                new JobType("Developer"),
                new JobType("Officer"),
                new JobType("Architect"),
                new JobType("Representative"),
                new JobType("Other"));
        }
    }
}
