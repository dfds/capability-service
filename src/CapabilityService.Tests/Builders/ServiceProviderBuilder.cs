using System;
using System.Linq;
using DFDS.CapabilityService.WebApi;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class ServiceProviderBuilder
    {
        readonly IServiceCollection _serviceCollection = new ServiceCollection();

        
        public ServiceProviderBuilder WithServicesFromStartup()
        {
            var configuration = new ConfigurationBuilder().Build();

            var startup = new Startup(configuration);
            startup.ConfigureServices(_serviceCollection);

            
            return this;
        }
        
        public ServiceProviderBuilder WithInMemoryDb()
        {
            RemoveDbContextOptionsBuilder();
            RemoveCapabilityServiceDbContext();
            AddInMemoryCapabilityServiceDbContext();
            
            return this;
        }
        public IServiceProvider Build()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
        
        
        private void RemoveCapabilityServiceDbContext()
        {
            var dbContextOptionsToRemove = _serviceCollection.FirstOrDefault(descriptor =>
                descriptor.ServiceType == typeof(CapabilityServiceDbContext)
            );
            if (dbContextOptionsToRemove != null) _serviceCollection.Remove(dbContextOptionsToRemove);
        }

        private void AddInMemoryCapabilityServiceDbContext()
        {
            _serviceCollection
                .AddEntityFrameworkNpgsql()
                .AddDbContext<CapabilityServiceDbContext>((serviceProvider, options) =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
        }

        private void RemoveDbContextOptionsBuilder()
        {
            var dbContextOptionsToRemove = _serviceCollection.FirstOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbContextOptions<CapabilityServiceDbContext>)
            );
            if (dbContextOptionsToRemove != null) _serviceCollection.Remove(dbContextOptionsToRemove);
        }
    }
}