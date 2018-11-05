using System.IO;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence;
using Xunit;
namespace DFDS.TeamService.Tests.Features.AwsRoles.Infrastructure.Pesistance.PolicyRepository
{
    public class ReadFileAsyncFacts
    {
        [Fact]
        public async Task Read_a_file()
        {            
            // Arrange
            var baseFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filesFolder = Path.Combine(baseFolder, "Features/AwsRoles/Infrastructure/Persistence/PolicyRepository/Policies");
            var policyDirectoryLocation = new PolicyDirectoryLocation(filesFolder);
            
            
            var policyRepository = new WebApi.Features.AwsRoles.Infrastructure.Persistence.PolicyRepository(policyDirectoryLocation);

            var filePath = Path.Combine(policyDirectoryLocation, "letsBeFriends.json");
            // Act
            var fileContent = await policyRepository.ReadFileAsync(filePath);
            
            
            // Assert
            Assert.NotNull(fileContent);
        }
    }
}