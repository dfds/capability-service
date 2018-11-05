using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using DFDS.TeamService.WebApi.Features.AwsRoles.Model;
using Moq;
using Xunit;

namespace DFDS.TeamService.IntegrationTests.Features
{
    public class AwsIdentityClientFacts
    {
        [Fact]
        public async Task HandleExistingRole()
        {
            // Arrange
            var awsCredentials = new EnvironmentVariablesAWSCredentials();

            var policyRepositoryBuilder = new Mock<IPolicyRepository>();
            policyRepositoryBuilder.Setup(p => p.GetLatestAsync()).ReturnsAsync(new Policy[0]);
            var awsIdentityClient = new AwsIdentityClient(
                awsCredentials,
                policyRepositoryBuilder.Object
            );


            var roleName = $"throw-away-{Guid.NewGuid()}";

            
            // Act
            try
            {
                await awsIdentityClient.PutRoleAsync(roleName);
                await awsIdentityClient.PutRoleAsync(roleName);
            }
            finally
            {
               await awsIdentityClient.DeleteRole(roleName);
            }
        }
    }
}