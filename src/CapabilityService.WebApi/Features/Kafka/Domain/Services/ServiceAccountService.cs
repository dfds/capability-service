using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using KafkaJanitor.RestClient;
using KafkaJanitor.RestClient.Features.Access.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services
{
	public class ServiceAccountService : IServiceAccountService
	{
		private readonly IRestClient _kafkaJanitorClient;

		public ServiceAccountService(IRestClient kafkaJanitorClient)
		{
			_kafkaJanitorClient = kafkaJanitorClient;
		}
		
		public async Task EnsureServiceAccountAvailability(Capability capability, Topic topic)
		{
			var payload = new ServiceAccountRequestInput
			{
				CapabilityName = capability.Name,
				CapabilityId = capability.Id.ToString()
			};
			
			await _kafkaJanitorClient.Access.RequestAsync(payload);
		}
	}
	
}
