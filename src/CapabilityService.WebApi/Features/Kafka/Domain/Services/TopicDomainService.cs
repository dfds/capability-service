using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services
{
	public class TopicDomainService : ITopicDomainService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;


		public TopicDomainService(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		public async Task CreateTopic(Topic topic, bool dryRun)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var topicRepository = scope.ServiceProvider.GetRequiredService<ITopicRepository>();
				var existingTopics = await topicRepository.GetAllAsync();
			
				if(existingTopics.Any(t => t.Name.Equals(topic.Name))){ throw new TopicAlreadyExistException(topic.Name);}

				if(dryRun) return;
			
				await topicRepository.AddAsync(topic);
			}
		}
	}
}
