using System.Threading.Tasks;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsRoles.Infrastructure.Pesistance.PolicyRepository
{
    public class ReadFileAsyncFacts
    {
        [Fact]
        public async Task Read_a_file()
        {
            // Arrange
            var policyRepository = new WebApi.Features.AwsRoles.Infrastructure.Persistence.PolicyRepository();
            
            
            // Act
            var fileContent = await policyRepository.ReadFileAsync("rds-all-v001.json");
            
            
            // Assert
            Assert.NotNull(fileContent);
        }
    }
}