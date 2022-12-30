using LoanComparer.Application.Model;
using LoanComparer.Application.ModelEntityTypeConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoanComparer.Application
{
    public class LoanComparerContext : IdentityDbContext<
        User,
        Role,
        string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public DbSet<JobType> JobTypes => Set<JobType>();

        public LoanComparerContext(DbContextOptions<LoanComparerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobTypeEntityConfiguration).Assembly);
        }
    }
}
