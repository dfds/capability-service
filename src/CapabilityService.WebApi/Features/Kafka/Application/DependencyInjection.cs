using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.RestClients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Application
{
	public static class DependencyInjection
	{
		public static void AddKafka(this IServiceCollection services, string dataBaseConnectionString)
		{
			services.TryAddTransient<ITopicRepository, EntityFrameworkTopicRepository>();
			services.TryAddTransient<IClusterRepository, EntityFrameworkClusterRepository>();
			services.TryAddTransient<ITopicDomainService, TopicDomainService>();
			services.TryAddTransient<IKafkaJanitorRestClient, KafkaJanitorRestClient>();
			services.AddSingleton<IKafkaDbContextFactory>(new KafkaDbContextFactory(dataBaseConnectionString));
			
			services
				.AddEntityFrameworkNpgsql()
				.AddDbContext<KafkaDbContext>((serviceProvider, options) =>
				{
					ConfigureDbOptions(options, dataBaseConnectionString);
				});
		}

		public static void ConfigureDbOptions(DbContextOptionsBuilder optionsBuilder, string databaseConnectionString)
		{
			optionsBuilder.UseNpgsql(databaseConnectionString);
		}
	}
}
