using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CapabilityService.IntegrationTests.Features.Shared
{
	public static class ApiClient
	{
		public static async Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequestMessage)
		{
			var httpClient = new HttpClient();
			var responseMessage = await httpClient.SendAsync(httpRequestMessage);

			if (responseMessage.IsSuccessStatusCode == false)
			{
				throw new Exception(responseMessage.ReasonPhrase);
			}

			return responseMessage;
		}


		public static HttpRequestMessage CreatePostHttpRequestMessage(Uri uri, object payload)
		{
			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

			httpRequestMessage.Content = new StringContent(
				JsonConvert.SerializeObject(payload),
				Encoding.UTF8,
				"application/json"
			);

			return httpRequestMessage;
		}
	}
}
