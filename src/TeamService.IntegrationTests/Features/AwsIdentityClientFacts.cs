using System;
using System.Threading.Tasks;
using Xunit;

namespace DFDS.TeamService.IntegrationTests.Features
{
    public class AwsIdentityClientFacts
    {
        [Fact]
        public async Task HandleExistingRole()
        {
            // Arrange
            var awsIdentityClient = AwsIdentityClientFactory.CreateFromEnviromentVariables();

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