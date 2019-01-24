using DFDS.TeamService.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DFDS.TeamService.WebApi.Persistence
{
    public class TeamServiceDbContext : DbContext
    {
        public TeamServiceDbContext(DbContextOptions<TeamServiceDbContext> options) : base(options)
        {
            
        }

        public DbSet<Team> Teams { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>(cfg =>
            {
                cfg.ToTable("Team");
                cfg.Ignore(x => x.Members);

                cfg.HasMany<Membership>(x => x.Memberships)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Membership>(cfg =>
            {
                cfg.ToTable("Membership");
                cfg.OwnsOne(x => x.Member)
                   .Property(x => x.Email)
                   .HasColumnName("MemberEmail");
            });
        }
    }
}