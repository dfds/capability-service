using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsRoles.Infrastructure.Pesistance.PolicyRepository
{
    public class GetAllPolicyFilesFacts
    {
        [Fact]
        public async Task Can_list_a_folder()
        {
            // Arrange
            var baseFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filesFolder = Path.Combine(baseFolder, "Features/AwsRoles/Infrastructure/Persistence/PolicyRepository/Policies/GetAllPolicyFilesFacts/Can_list_a_folder");
            var policyDirectoryLocation = new PolicyDirectoryLocation(filesFolder);
            
            
            var policyRepository = new WebApi.Features.AwsRoles.Infrastructure.Persistence.PolicyRepository(policyDirectoryLocation);

            
            // Act
            var files = policyRepository.GetAllPolicyFiles();
            
            
            // Assert 
            Assert.Equal(2, files.Count());

            var expectedFile1 = Path.Combine(policyDirectoryLocation, "file1.json");
            Assert.Contains(expectedFile1, files);
            
            var expectedFile2 = Path.Combine(policyDirectoryLocation, "file2.json");
            Assert.Contains(expectedFile2, files);
        }
    }
}