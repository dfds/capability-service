using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Application;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Api;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Api.DTOs;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Capability = DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models.Capability;

namespace DFDS.CapabilityService.Tests.Scenarios
{
    public class DeleteCapabilityScenario
    {
        private Capability _capability;
        private ICapabilityApplicationService _capabilityApplicationService;
        private IServiceProvider _serviceProvider;


        [Fact]
        public async Task DeleteCapabilityRecipe()
        {
                  Given_a_service_collection_with_a_imMemoryDb();
            await And_a_existing_capability();
            await When_delete_capability_is_posted();
            await Then_the_given_capability_will_not_be_listed_in_api();
            await And_a_capability_deleted_event_is_outboxed();
        }

        private void Given_a_service_collection_with_a_imMemoryDb()
        {
            var serviceProviderBuilder = new ServiceProviderBuilder();

            _serviceProvider = serviceProviderBuilder
                .WithServicesFromStartup()
                .WithInMemoryDb()
                .Build();

            _capabilityApplicationService = _serviceProvider.GetService<ICapabilityApplicationService>();
        }

        private async Task And_a_existing_capability()
        {
            _capability = await _capabilityApplicationService.CreateCapability("Foo", "This is a capability");
        }

        private async Task When_delete_capability_is_posted()
        {
            var capabilityController = new CapabilityController(_capabilityApplicationService);

            await capabilityController.DeleteCapability(_capability.Id.ToString());
        }

        private async Task Then_the_given_capability_will_not_be_listed_in_api()
        {
            var capabilityController = new CapabilityController(_capabilityApplicationService);
            var actionResult = await capabilityController.GetAllCapabilities();
            var objectResult = actionResult as OkObjectResult;
            var capabilityResponse = objectResult.Value as CapabilityResponse;

            Assert.Empty(capabilityResponse.Items);
        }

        private async Task And_a_capability_deleted_event_is_outboxed()
        {
            var domainEventEnvelopeRepository = _serviceProvider.GetService<IRepository<DomainEventEnvelope>>();
            var outBoxedEvents = await domainEventEnvelopeRepository.GetAll();

            outBoxedEvents.Single(envelope =>
                envelope.Type== "capability_deleted"
            );
        }
    }
}