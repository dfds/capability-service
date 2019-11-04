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

            public static async Task<CapabilityDto> GetAsync(Guid capabilityId)
            {
                var uri = new Uri(CapabilitiesUrl + "/" + capabilityId);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                var responseMessage = await SendRequest(httpRequestMessage);

                return await DeserializeContent<CapabilityDto>(responseMessage);
            }


            public static async Task<ItemsEnvelope<CapabilityDto>> GetAsync()
            {
                var uri = new Uri(CapabilitiesUrl);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                var responseMessage = await SendRequest(httpRequestMessage);

                return await DeserializeContent<ItemsEnvelope<CapabilityDto>>(responseMessage);
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

                var responseMessage = await SendRequest(httpRequestMessage);

                return await DeserializeContent<CapabilityDto>(responseMessage);
            }


            public static async Task DeleteAsync(Guid capabilityId)
            {
                var uri = new Uri(CapabilitiesUrl + "/" + capabilityId);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                await SendRequest(httpRequestMessage);
            }

            
            private static async Task<T> DeserializeContent<T>(HttpResponseMessage httpResponseMessage)
            {
                var contentString = await httpResponseMessage.Content.ReadAsStringAsync();


                var deserializeObject = JsonConvert.DeserializeObject<T>(contentString);
                return deserializeObject;
            }

            private static async Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequestMessage)
            {
                var httpClient = new HttpClient();
                var responseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (responseMessage.IsSuccessStatusCode == false)
                {
                    throw new Exception(responseMessage.ReasonPhrase);
                }

                return responseMessage;
            }
        }
    }
}