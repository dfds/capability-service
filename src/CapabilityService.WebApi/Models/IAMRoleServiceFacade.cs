using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.CapabilityService.WebApi.Models
{
    public class IAMRoleServiceFacade
    {
        private readonly HttpClient _client;

        public IAMRoleServiceFacade(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<string> CreateRole(string capabilityName)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            
            var requestPayload = new StringContent(
                content: JsonConvert.SerializeObject(new {Name = capabilityName}, serializerSettings),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );
            
            var response = await _client.PostAsync("/api/roles", requestPayload);
            response.EnsureSuccessStatusCode();

            var responsePayload = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RoleInformation>(responsePayload);

            return result.RoleArn;
        }

        private class RoleInformation
        {
            public string RoleArn { get; set; }
        }
    }
}