using System;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Api.DTOs
{
    public class TestCapability
    {
        [Fact]
        public void create_will_set_all_fields()
        {
            // Arrange
            var capabilityId = Guid.NewGuid();
            var capabilityName = "foo";
            var domainMembership = new Membership(Guid.NewGuid(), new Member("fu@baa.com"));
            var domainContext = new Context(Guid.NewGuid(), "baa");
            var domainCapability = new Capability(
                capabilityId,
                capabilityName,
                new []{domainMembership},
                new []{domainContext}
            );
            
            
            // Act
            var capabilityDto = WebApi.Infrastructure.Api.DTOs.Capability.Create(domainCapability);
            
            // Assert
            Assert.Equal(capabilityId, capabilityDto.Id);
            Assert.Equal(capabilityName, capabilityDto.Name);

            var membershipDto = capabilityDto.Members.Single();
            Assert.Equal(domainMembership.Member.Email, membershipDto.Email);

            var contextDto = capabilityDto.Contexts.Single();
            Assert.Equal(domainContext.Id, contextDto.Id);
            Assert.Equal(domainContext.Name, contextDto.Name);
        }
    }
}