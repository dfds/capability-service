using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Factories;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;
namespace DFDS.CapabilityService.Tests.Application
{
    public class TestCapabilityApplicationService
    {
        [Fact]
        public async Task expected_member_is_added_to_capability()
        {
            var capability = new CapabilityBuilder().Build();

            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository(capability))
                .Build();

            var stubMemberEmail = "foo@bar.com";
            
            await sut.JoinCapability(capability.Id, stubMemberEmail);
            
            Assert.Equal(new[]{stubMemberEmail}, capability.Members.Select(x => x.Email));
        }

        [Fact]
        public async Task throws_exception_with_underscore_in_name()
        {
            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository())
                .WithCapabilityFactory(new CapabilityFactory())
                .Build();

            var dummyName = "A_Name";

            await Assert.ThrowsAsync<CapabilityValidationException>(() => sut.CreateCapability(dummyName, string.Empty));
        }

        [Fact]
        public async Task throws_exception_when_adding_member_to_a_non_existing_capability()
        {
            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository())
                .Build();

            var nonExistingCapabilityId = Guid.Empty;
            var dummyMemberEmail = "foo@bar.com";

            await Assert.ThrowsAsync<CapabilityDoesNotExistException>(() => sut.JoinCapability(nonExistingCapabilityId, dummyMemberEmail));
        }

        [Fact]
        public async Task adding_the_same_member_to_a_capability_multiple_times_yields_single_membership()
        {
            var capability = new CapabilityBuilder().Build();

            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository(capability))
                .Build();

            var stubMemberEmail = "foo@bar.com";
            
            await sut.JoinCapability(capability.Id, stubMemberEmail);
            await sut.JoinCapability(capability.Id, stubMemberEmail);
            
            Assert.Equal(new[]{stubMemberEmail}, capability.Members.Select(x => x.Email));
        }

        [Fact]
        public async Task expected_member_is_removed_from_capability()
        {
            var stubMemberEmail = "foo@bar.com";

            var capability = new CapabilityBuilder()
                .WithMembers(stubMemberEmail)
                .Build();

            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository(capability))
                .Build();

            await sut.LeaveCapability(capability.Id, stubMemberEmail);

            Assert.Empty(capability.Members);
        }

        [Fact]
        public async Task removing_non_existing_member_from_a_capability_throws_exception()
        {
            var capability = new CapabilityBuilder().Build();

            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository(capability))
                .Build();

            var nonExistingMemberEmail = "foo@bar.com";

            await Assert.ThrowsAsync<NotMemberOfCapabilityException>(() => sut.LeaveCapability(capability.Id, nonExistingMemberEmail));
        }

        [Fact]
        public async Task removing_member_from_non_existing_capability_throws_exception()
        {
            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository())
                .Build();

            var nonExistingCapabilityId = Guid.Empty;
            var dummyMemberId = "foo@bar.com";

            await Assert.ThrowsAsync<CapabilityDoesNotExistException>(() => sut.LeaveCapability(nonExistingCapabilityId, dummyMemberId));
        }

        [Fact]
        public async Task adding_a_context_to_a_non_existing_capability_throws_exception()
        {
            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository())
                .Build();

            var nonExistingCapabilityId = Guid.Empty;
            var dummyMemberId = "foo@bar.com";

            await Assert.ThrowsAsync<CapabilityDoesNotExistException>(() => sut.AddContext(nonExistingCapabilityId, dummyMemberId));

        }
        
        [Fact]
        public async Task updating_a_non_existing_context_on_an_existing_capability_throws_exception()
        {
            var capability = new CapabilityBuilder()
                .Build();
            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository(capability))
                .Build();

            var nonExistingContextId = Guid.Empty;
            var dummyAccountId = "111111111111";
            var dummyEmail = "dummy@mail.com";
            var dummyArn = "mynameisarne";
            

            await Assert.ThrowsAsync<ContextDoesNotExistException>(() => 
                sut.UpdateContext(capability.Id.ToString(), nonExistingContextId.ToString(), dummyAccountId, dummyArn,dummyEmail));
        }
        
        [Fact]
        public async Task updating_a_existing_context_on_an_existing_capability()
        {
            var context = new Context(Guid.NewGuid(), "default");
            var capability = new CapabilityBuilder()
                .WithContexts(context)
                .Build();
            
            var sut = new CapabilityApplicationServiceBuilder()
                .WithCapabilityRepository(new StubCapabilityRepository(capability))
                .Build();

            var newAccountId = "111111111111";
            var newEmail = "dummy@mail.com";
            var newRoleArn = "mynameisarne";

            await sut.UpdateContext(capability.Id.ToString(), context.Id.ToString(), newAccountId, newRoleArn, newEmail);

            var contextFromCapability = capability.Contexts.Single();
            Assert.Equal(contextFromCapability.AWSAccountId, newAccountId);
            Assert.Equal(contextFromCapability.AWSRoleArn, newRoleArn);
            Assert.Equal(contextFromCapability.AWSRoleEmail, newEmail);
        }
    }
}