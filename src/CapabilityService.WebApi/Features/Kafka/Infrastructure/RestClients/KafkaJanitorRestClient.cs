using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using KafkaJanitor.RestClient;
using KafkaJanitor.RestClient.Features.Access.Models;
using Topic = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models.Topic;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.RestClients
{
	public class KafkaJanitorRestClient : IKafkaJanitorRestClient
	{
		private readonly IRestClient _kafkaJanitorClient;

		public KafkaJanitorRestClient(IRestClient kafkaJanitorClient)
		{
			_kafkaJanitorClient = kafkaJanitorClient;
		}

		public async Task CreateTopic(Topic topic, Capability capability)
		{
			
			await _kafkaJanitorClient.Topics.CreateAsync(new KafkaJanitor.RestClient.Features.Topics.Models.Topic
			{
				Name = topic.Name.Name, 
				Description = topic.Description, 
				Partitions = topic.Partitions
			});

			await _kafkaJanitorClient.Access.RequestAsync(
				new ServiceAccountRequestInput
				{
					CapabilityName = capability.Name, 
					CapabilityId = capability.Id.ToString(),
					CapabilityRootId = capability.RootId
				}
			);
		}
	}
}
