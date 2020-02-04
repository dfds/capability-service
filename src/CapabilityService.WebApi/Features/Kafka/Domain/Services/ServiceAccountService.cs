using System.Net.Http;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services
{
	public class ServiceAccountService : IServiceAccountService
	{
		private readonly HttpClient _httpClient;

		public ServiceAccountService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		
		public async Task EnsureServiceAccountAvailability(Capability capability, Topic topic)
		{
			var req = await _httpClient.PostAsync("http://localhost:5000/api/serviceaccount/request", new StringContent(""));
		}
	}
}
