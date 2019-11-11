using System;
using System.Threading.Tasks;
using CapabilityService.IntegrationTests.Features.Shared;

namespace CapabilityService.IntegrationTests.Features.Topics.Infrastructure.Api
{
	public class TopicApiClient
	{
		public static async Task CreateTopic(
			Guid capabilityId,
			string name,
			string description
		)
		{
			var uri = new Uri(Urls.CapabilitiesUrl + "/" + capabilityId + "/topics");
			
			var payload = new
			{
				name = name,
				description = description,
				isPrivate = "true"
			};

			var httpRequestMessage = ApiClient.CreatePostHttpRequestMessage(uri, payload);
			
			await ApiClient.SendRequest(httpRequestMessage);
		}
	}
}
