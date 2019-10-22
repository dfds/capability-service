using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Infrastructure.Api;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Capability = DFDS.CapabilityService.WebApi.Domain.Models.Capability;

namespace DFDS.CapabilityService.Tests.Scenarios
{
    public class TwoCapabilitiesWithTheSameName
    {
        private Capability _capability;
        private ICapabilityApplicationService _capabilityApplicationService;
        private IServiceProvider _serviceProvider;
        private IActionResult _createResponse;


        [Fact]
        public async Task TwoCapabilitiesWithTheSameNameRecipe()
        {
            Given_a_service_collection_with_a_imMemoryDb();
            await And_a_existing_capability();
            await When_a_capability_create_is_posted(); 
                  Then_a_conflict_response_is_given();
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
        
        private async Task When_a_capability_create_is_posted()
        {
            var capabilityController = new CapabilityController(_capabilityApplicationService);

            var capabilityInput = new CapabilityInput
            {
                Name = _capability.Name,
                Description = _capability.Description
            };
            
            _createResponse =   await capabilityController.CreateCapability(capabilityInput);
        }

        private void Then_a_conflict_response_is_given()
        {
            Assert.IsType<ConflictObjectResult>(_createResponse);
        }
    }
}