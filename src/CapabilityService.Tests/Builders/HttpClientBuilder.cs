using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DFDS.CapabilityService.Tests.Builders
{
	public class HttpClientBuilder : IDisposable
	{
		private readonly LinkedList<IDisposable> _disposables = new LinkedList<IDisposable>();

		private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors =
			new Dictionary<Type, ServiceDescriptor>();

		public HttpClientBuilder WithService(Type serviceType, object serviceInstance)
		{
			_serviceDescriptors.Remove(serviceType);
			_serviceDescriptors.Add(serviceType, ServiceDescriptor.Singleton(serviceType, serviceInstance));

			return this;
		}

		public HttpClientBuilder WithService<TService>(TService serviceInstance)
		{
			return WithService(typeof(TService), serviceInstance);
		}

		public HttpClientBuilder WithService<TServiceContract, TServiceImplementation>()
		{
			var serviceType = typeof(TServiceContract);

			_serviceDescriptors.Remove(serviceType);
			_serviceDescriptors.Add(serviceType,
				ServiceDescriptor.Transient(serviceType, typeof(TServiceImplementation)));

			return this;
		}

		private IWebHostBuilder CreateWebHostBuilder()
		{
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string>()
				{
					{"CAPABILITY_SERVICE_KAFKA_BOOTSTRAP_SERVERS", "localhost:9092"},
					{"CAPABILITY_SERVICE_KAFKA_GROUP_ID", "capability-service-consumer"},
					{"CAPABILITY_SERVICE_KAFKA_TOPIC_CAPABILITY", "build.selfservice.events.capabilities"},
					{"CAPABILITY_SERVICE_KAFKA_TOPIC_TOPICS", "build.selfservice.events.topics"},
					{"CAPABILITY_SERVICE_KAFKA_TOPIC_SELF_SERVICE", "cloudengineering.selfservice.kafkatopic"},
					{"CAPABILITY_SERVICE_KAFKA_TOPIC_CONFLUENT", "cloudengineering.confluentgateway.provisioning"}
				})
				.Build();

			var testWebHostBuilder = WebHost.CreateDefaultBuilder()
				.UseStartup<TestAuthStartUp>()
				.UseConfiguration(configuration)
				.UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).Assembly.GetName().Name) // We need to look for controllers in the same assembly as the startup file
				.ConfigureTestServices(services =>
				{
					_serviceDescriptors
						.Values
						.ToList()
						.ForEach(serviceOverride => services.Replace(serviceOverride));
				});
			return testWebHostBuilder;
		}

		private List<Action<HttpClient>> CreateCustomizations()
		{
			var customizations = new List<Action<HttpClient>>();

			customizations.Add(client =>
			{
				client.Timeout = TimeSpan.FromMinutes(1);
			});

			return customizations;
		}

		public HttpClient Build()
		{
			var webHostBuilder = CreateWebHostBuilder();
			var testServer = new TestServer(webHostBuilder);
			_disposables.AddLast(testServer);

			var customizations = CreateCustomizations();
			var client = testServer.CreateClient();

			customizations.ForEach(x => x(client));
			_disposables.AddLast(client);

			return client;
		}

		public void Dispose()
		{
			foreach (var instance in _disposables.Reverse())
			{
				instance.Dispose();
			}
		}
	}
}
