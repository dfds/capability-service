using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Newtonsoft.Json;

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
			var payload = new ServiceAccountRequestInput
			{
				CapabilityName = capability.Name,
				CapabilityId = capability.Id.ToString()
			};
			
			var req = await _httpClient.PostAsync("http://localhost:5000/api/serviceaccount/request", new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
		}
	}
	
	public class ServiceAccountRequestInput
	{
		public string CapabilityName { get; set; }
		public string CapabilityId { get; set; }
	}
}
