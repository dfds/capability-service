using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api
{
    public static class CapabilityApiClient
    {
         public static class Capabilities
         {
             private const string CapabilitiesUrl = "http://localhost:50900/api/v1/capabilities";

             public static async Task GetAsync()
             {
                 
             }

             public static async Task PostAsync(string name, string description)
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
             }
         } 
    }
}