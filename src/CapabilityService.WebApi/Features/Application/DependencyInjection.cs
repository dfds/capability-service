using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DFDS.CapabilityService.WebApi.Features.Kafka
{
	public static class DependencyInjection
	{
		public static void AddKafka(this IServiceCollection services, string dataBaseConnectionString)
		{
			services.TryAddTransient<ITopicRepository,EntityFrameworkTopicRepository>();
			services.TryAddTransient<ITopicDomainService,TopicDomainService>();
				
			services
				.AddEntityFrameworkNpgsql()
				.AddDbContext<KafkaDbContext>((serviceProvider, options) =>
				{
					options.UseNpgsql(dataBaseConnectionString);
				});
		}
	}
}
