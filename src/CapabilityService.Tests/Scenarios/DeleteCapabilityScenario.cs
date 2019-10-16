using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DFDS.CapabilityService.Tests.Scenarios
{
    public class DeleteCapabilityScenario
    {
        private Capability _capability;
        private ICapabilityApplicationService _capabilityApplicationService;


        [Fact]
        public async Task DeleteCapabilityRecipe()
        {
            await Given_a_service_collection_with_a_imMemoryDb();
            await And_a_existing_capability();
            await When_delete_capability_is_posted();
            Then_capability_deleted_event_is_emitted();
            And_capability_is_deleted_from_database();
        }

        private async Task Given_a_service_collection_with_a_imMemoryDb()
        {
            var serviceProviderBuilder = new ServiceProviderBuilder();
           
            var serviceProvider = serviceProviderBuilder
                .WithServicesFromStartup()
                .WithInMemoryDb()
                .Build();

            _capabilityApplicationService = serviceProvider.GetService<ICapabilityApplicationService>();

        }

        private async Task And_a_existing_capability()
        {
            _capability = await _capabilityApplicationService.CreateCapability("Foo", "This is a capability");
        }

        private async Task When_delete_capability_is_posted()
        {
        }

        private void Then_capability_deleted_event_is_emitted()
        {
        }

        private void And_capability_is_deleted_from_database()
        {
        }
        
    
    }
}