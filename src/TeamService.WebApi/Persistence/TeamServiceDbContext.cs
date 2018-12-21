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
            });

            modelBuilder.Entity<Member>(cfg =>
            {
                cfg.ToTable("Member");
                cfg.HasKey(x => x.Email);
            });
        }
    }
}