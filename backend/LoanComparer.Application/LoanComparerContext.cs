using LoanComparer.Application.Model;
using LoanComparer.Application.ModelEntityTypeConfiguration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application
{
    public class LoanComparerContext : IdentityDbContext<User>
    {
        public DbSet<JobType> JobTypes { get; private set; }

        public LoanComparerContext(DbContextOptions<LoanComparerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobTypeEntityConfiguration).Assembly);

            modelBuilder.Entity<JobType>().HasData( // TODO
                new JobType("JobType1"),
                new JobType("JobType2"),
                new JobType("Other"));
        }
    }
}
