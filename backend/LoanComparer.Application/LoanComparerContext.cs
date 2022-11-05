using LoanComparer.Application.Model;
using LoanComparer.Application.ModelEntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application
{
    public class LoanComparerContext : DbContext
    {
        public DbSet<JobType> JobTypes { get; private set; }

        public LoanComparerContext(DbContextOptions<LoanComparerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobTypeEntityConfiguration).Assembly);

            modelBuilder.Entity<JobType>().HasData( // TODO
                new JobType("JobType1"),
                new JobType("JobType2"),
                new JobType("Other"));
        }
    }
}
