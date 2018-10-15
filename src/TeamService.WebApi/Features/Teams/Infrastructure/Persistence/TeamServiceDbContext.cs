using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Persistence
{
    public class TeamServiceDbContext : DbContext
    {
        public TeamServiceDbContext(DbContextOptions<TeamServiceDbContext> options) : base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>(cfg =>
            {
                cfg.ToTable("Team");

                //cfg.HasKey(x => x.Id);
                cfg.Property(x => x.Name).IsRequired();
                cfg.Property(x => x.Department).IsRequired();

                cfg.Ignore(x => x.Members);

                cfg
                    .HasMany(x => x.Memberships)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Membership>(cfg =>
            {
                cfg.ToTable("Membership");

                //cfg.HasKey(x => x.Id);
                cfg.HasOne(x => x.User);
                cfg.Property(x => x.Type).HasConversion<string>().IsRequired();
                cfg.Property(x => x.StartedDate).IsRequired();
            });

            modelBuilder.Entity<User>(cfg =>
            {
                cfg.ToTable("User");

                //cfg.HasKey(x => x.Id);
                cfg.Property(x => x.Name).IsRequired();
                cfg.Property(x => x.Email).IsRequired();
            });
        }
    }
}