using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Api;
using KafkaJanitor.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DFDS.CapabilityService.Tests.Features.Kafka.Infrastructure.Api
{
	public class TestClusterRoute
	{
		[Fact]
		public async Task get_all_clusters_returns_expected_status_code()
		{
				var serviceProviderBuilder = new ServiceProviderBuilder();
				var serviceProvider = serviceProviderBuilder
					.WithServicesFromStartup()
					.WithInMemoryDb()
					.OverwriteService(typeof(IRestClient), new StubKafkaRestClient())
					.Build();
				var clusterController = new ClusterController(serviceProvider.GetService<IClusterRepository>());

				var response = await clusterController.GetAll();
				var okResult = response as ObjectResult;

				Assert.Equal(HttpStatusCode.OK.GetHashCode(), okResult.StatusCode);
		}
		
		[Fact]
		public async Task get_all_clusters_returns_expected_body_when_no_clusters_available()
		{
			var serviceProviderBuilder = new ServiceProviderBuilder();
			var serviceProvider = serviceProviderBuilder
				.WithServicesFromStartup()
				.WithInMemoryDb()
				.OverwriteService(typeof(IRestClient), new StubKafkaRestClient())
				.Build();
			var clusterController = new ClusterController(serviceProvider.GetService<IClusterRepository>());

			var response = await clusterController.GetAll();
			var okResult = response as OkObjectResult;

			Assert.Equal(
				expected: "[]",
				actual: JsonSerializer.Serialize(okResult.Value)
			);
		}
		
		[Fact]
		public async Task get_all_clusters_returns_expected_body_with_clusters_available()
		{
			var serviceProviderBuilder = new ServiceProviderBuilder();
			var serviceProvider = serviceProviderBuilder
				.WithServicesFromStartup()
				.WithInMemoryDb()
				.OverwriteService(typeof(IRestClient), new StubKafkaRestClient())
				.Build();
			var clusterController = new ClusterController(serviceProvider.GetService<IClusterRepository>());
			
			var clusterRepository = serviceProvider.GetService<IClusterRepository>();
			await clusterRepository.AddAsync("Dummy test cluster #1", "lkc-9999", true, Guid.Parse("1d569b07-75b4-4e2f-b067-6583401a09c8"));
			await clusterRepository.AddAsync("Dummy test cluster #2", "lkc-9998", true, Guid.Parse("b8c32078-9eec-4809-8f4e-5efbe509b78f"));
			
			var response = await clusterController.GetAll();
			var okResult = response as OkObjectResult;

			Assert.Equal(
				expected: "[{\"Name\":\"Dummy test cluster #1\",\"Description\":\"\",\"Enabled\":true,\"ClusterId\":\"lkc-9999\",\"DomainEvents\":[],\"Id\":\"1d569b07-75b4-4e2f-b067-6583401a09c8\"},{\"Name\":\"Dummy test cluster #2\",\"Description\":\"\",\"Enabled\":true,\"ClusterId\":\"lkc-9998\",\"DomainEvents\":[],\"Id\":\"b8c32078-9eec-4809-8f4e-5efbe509b78f\"}]",
				actual: JsonSerializer.Serialize(okResult.Value)
			);
		}
	}
}
