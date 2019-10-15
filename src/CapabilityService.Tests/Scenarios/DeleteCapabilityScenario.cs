using Xunit;

namespace DFDS.CapabilityService.Tests.Scenarios
{
    public class DeleteCapabilityScenario
    {
        [Fact]
        public void DeleteCapabilityRecipe()
        {
            Given_a_existing_capability();
            When_delete_capability_is_posted();
            Then_capability_deleted_event_is_emitted();
            And_capability_is_deleted_from_database();
        }

        private void Given_a_existing_capability()
        {
            throw new System.NotImplementedException();
        }

        private void When_delete_capability_is_posted()
        {
            throw new System.NotImplementedException();
        }

        private void Then_capability_deleted_event_is_emitted()
        {
            throw new System.NotImplementedException();
        }

        private void And_capability_is_deleted_from_database()
        {
            throw new System.NotImplementedException();
        }
    }
}