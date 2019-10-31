using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api.Model;
using Newtonsoft.Json;

namespace CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api
{
    public static class CapabilityApiClient
    {
        public static class Capabilities
        {
            private const string CapabilitiesUrl = "http://localhost:50900/api/v1/capabilities";

            public static async Task<ItemsEnvelope<CapabilityDto>> GetAsync()
            {
                var uri = new Uri(CapabilitiesUrl);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                var httpClient = new HttpClient();
                var responseMessage = await httpClient.SendAsync(httpRequestMessage);

                var contentString = await responseMessage.Content.ReadAsStringAsync();

                var deserializeObject = JsonConvert.DeserializeObject<ItemsEnvelope<CapabilityDto>>(contentString);

                return deserializeObject;
            }

            public static async Task<CapabilityDto> PostAsync(string name, string description)
            {
                var payload = new
                {
                    name = name,
                    description = description
                };


                var uri = new Uri(CapabilitiesUrl);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

                httpRequestMessage.Content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );


                var httpClient = new HttpClient();
                var responseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (responseMessage.IsSuccessStatusCode == false)
                {
                    throw new Exception(responseMessage.ReasonPhrase);
                }

                var contentString = await responseMessage.Content.ReadAsStringAsync();

                var deserializeObject = JsonConvert.DeserializeObject<CapabilityDto>(contentString);

                return deserializeObject;
            }

            public static async Task DeleteAsync(Guid capabilityId)
            {
                var uri = new Uri(CapabilitiesUrl + "/" + capabilityId);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                var httpClient = new HttpClient();
                var responseMessage = await httpClient.SendAsync(httpRequestMessage);


                if (responseMessage.IsSuccessStatusCode == false)
                {
                    throw new Exception(responseMessage.ReasonPhrase);
                }
            }
        }
    }
}