using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaJanitor.RestClient;
using KafkaJanitor.RestClient.Features.Access;
using KafkaJanitor.RestClient.Features.Access.Models;
using KafkaJanitor.RestClient.Features.Topics;
using KafkaJanitor.RestClient.Features.Topics.Models;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
	public class StubKafkaRestClient : IRestClient
	{
		public ITopicsClient Topics { get; } = new StubTopicsClient();
		public IAccessClient Access { get; } = new StubAccessClient();
	}
	
	class StubTopicsClient : ITopicsClient
	{
		public Task CreateAsync(Topic input)
		{
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Topic>> GetAllAsync()
		{
			return Task.FromResult(Enumerable.Empty<Topic>());
		}
	}
	
	class StubAccessClient : IAccessClient
	{
		public Task RequestAsync(ServiceAccountRequestInput input)
		{
			return Task.CompletedTask;
		}
	}
}
