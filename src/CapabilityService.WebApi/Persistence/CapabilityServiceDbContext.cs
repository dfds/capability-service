using DFDS.CapabilityService.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Persistence
{
    public class CapabilityServiceDbContext : DbContext
    {
        public CapabilityServiceDbContext(DbContextOptions<CapabilityServiceDbContext> options) : base(options)
        {
            
        }

        public DbSet<Capability> Capabilities { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Capability>(cfg =>
            {
                cfg.ToTable("Capability");
                cfg.Ignore(x => x.Members);
                cfg.Ignore(x => x.DomainEvents);

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