using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.CapabilityService.WebApi.Models
{
    public class RoleMapperServiceFacade
    {
        private readonly HttpClient _client;

        public RoleMapperServiceFacade(HttpClient client)
        {
            _client = client;
        }

        public async Task CreateRoleMapping(string capabilityName, string roleIdentifier)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var payload = new StringContent(
                content: JsonConvert.SerializeObject(new {RoleName = capabilityName, RoleArn = roleIdentifier}, serializerSettings),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );
            
            var response = await _client.PostAsync("/api/roles", payload);
            response.EnsureSuccessStatusCode();
        }
    }
}