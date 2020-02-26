using System;
using System.Linq;
using DFDS.CapabilityService.Tests.Features.Kafka.Persistence;
using DFDS.CapabilityService.Tests.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence;
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

		public ServiceProviderBuilder OverwriteService(Type serviceType, object serviceInstance)
		{
			RemoveFromServiceCollection(serviceType);

			_serviceCollection.AddSingleton(serviceType, serviceInstance);
			return this;
		}

		public ServiceProviderBuilder WithServicesFromStartup()
		{
			var configuration = new ConfigurationBuilder().Build();

			var startup = new Startup(configuration);
			startup.ConfigureServices(_serviceCollection);


			return this;
		}

		public ServiceProviderBuilder WithInMemoryDb()
		{
			RemoveFromServiceCollection(typeof(DbContextOptions<CapabilityServiceDbContext>));
			RemoveFromServiceCollection(typeof(ICapabilityServiceDbContextFactory));
			RemoveFromServiceCollection(typeof(CapabilityServiceDbContext));
			AddInMemoryCapabilityServiceDbContext();

			RemoveFromServiceCollection(typeof(DbContextOptions<KafkaDbContext>));
			RemoveFromServiceCollection(typeof(IKafkaDbContextFactory));
			RemoveFromServiceCollection(typeof(KafkaDbContext));
			AddInMemoryKafkaDbContext();
			return this;
		}

		public IServiceProvider Build()
		{
			var serviceProvider = _serviceCollection.BuildServiceProvider();

			return serviceProvider;
		}

		private void AddInMemoryCapabilityServiceDbContext()
		{
			var id = Guid.NewGuid().ToString();

			_serviceCollection
				.AddSingleton<ICapabilityServiceDbContextFactory>(new CapabilityServiceDbContextInMemory(id))
				.AddEntityFrameworkNpgsql()
				.AddDbContext<CapabilityServiceDbContext>((serviceProvider, options) =>
				{
					options.UseInMemoryDatabase(id);
					options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
				});
		}

		private void AddInMemoryKafkaDbContext()
		{
			var id = Guid.NewGuid().ToString();

			_serviceCollection
				.AddSingleton<IKafkaDbContextFactory>(new KafkaDbContextInMemory(id))
				.AddEntityFrameworkNpgsql()
				.AddDbContext<KafkaDbContext>((serviceProvider, options) =>
				{
					options.UseInMemoryDatabase(id);
					options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
				});
		}

		private void RemoveFromServiceCollection(Type dbContextOptionsType)
		{
			var dbContextOptionsToRemove = _serviceCollection.FirstOrDefault(descriptor =>
				descriptor.ServiceType == dbContextOptionsType
			);
			if (dbContextOptionsToRemove != null) _serviceCollection.Remove(dbContextOptionsToRemove);
		}
	}
}
