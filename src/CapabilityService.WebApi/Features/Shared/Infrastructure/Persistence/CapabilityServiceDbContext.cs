using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Persistence
{
    public class CapabilityServiceDbContext : DbContext
    {
        public CapabilityServiceDbContext(DbContextOptions<CapabilityServiceDbContext> options) : base(options)
        {
            
        }

        public DbSet<Capability> Capabilities { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<DomainEventEnvelope> DomainEvents { get; set; }
        
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
                
                cfg.HasMany<Context>(x => x.Contexts)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Context>(cfg =>
            {
                cfg.ToTable("Context");
                cfg.Property(x => x.Name).HasColumnName("Name");
            });

            modelBuilder.Entity<Membership>(cfg =>
            {
                cfg.ToTable("Membership");
                cfg.OwnsOne(x => x.Member)
                   .Property(x => x.Email)
                   .HasColumnName("MemberEmail");
            });

            modelBuilder.Entity<DomainEventEnvelope>(cfg =>
            {
                cfg.HasKey(x => x.EventId);
                cfg.ToTable("DomainEvent");
            });

            modelBuilder.Entity<Topic>(cfg =>
            {
                cfg.ToTable("Topic");
                cfg.Ignore(x => x.DomainEvents);
                cfg.HasMany<MessageContract>(x => x.MessageContracts)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MessageContract>(cfg =>
            {
                cfg.ToTable("MessageContract");
                cfg.HasKey(x => x.Type);
            });
        }
    }
}