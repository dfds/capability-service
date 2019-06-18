using System;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Api.DTOs
{
    public class TestCapabilityADSync
    {
        [Fact]
        public void create_will_set_all_fields()
        {
            // Arrange
            var capabilityId = Guid.NewGuid();
            var capabilityName = "foo";
            var capabilityRootId = "foo-1234556";
            var capabilityDescription = "bar";
            var awsAccountId = "2222222222222";
            var awsRoleArn = "arn:aws:iam::528563840976:role/ADFS-Developer";
            var awsRoleEmail = "ADFS-Developer@dfds.cloud";
            var domainMembership = new Membership(Guid.NewGuid(), new Member("fu@baa.com"));
            var domainContext = new Context(Guid.NewGuid(), "baa", awsAccountId, awsRoleArn, awsRoleEmail);
            var domainCapability = new Capability(
                capabilityId,
                capabilityName,
                capabilityRootId,
                capabilityDescription,
                new []{domainMembership},
                new []{domainContext}
            );
            
            // Act
            var capabilityDto = WebApi.Infrastructure.Api.DTOs.CapabilityADSync.Create(domainCapability);
            
            // Assert
            Assert.Equal(capabilityName, capabilityDto.Identifier);
            Assert.Equal(true, capabilityDto.IsV1);
            Assert.Null(capabilityDto.AWSRoleArn);
            Assert.Null(capabilityDto.AWSAccountId);

            var membershipDto = capabilityDto.Members.Single();
            Assert.Equal(domainMembership.Member.Email, membershipDto.Email);
        }
    }
}