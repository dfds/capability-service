using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsRoles.Infrastructure.Pesistance.PolicyRepository.Policies
{
    public class GetLatestAsyncFacts
    {
        [Fact]
        public async Task Can_read_multiple_files()
        {
            // Arrange
            var baseFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filesFolder = Path.Combine(baseFolder, "Features/AwsRoles/Infrastructure/Persistence/PolicyRepository/Policies/GetLatestAsyncFacts/Can_read_multiple_files");
            var policyDirectoryLocation = new PolicyDirectoryLocation(filesFolder);
            
            
            var policyRepository = new WebApi.Features.AwsRoles.Infrastructure.Persistence.PolicyRepository(policyDirectoryLocation);

            
            // Act
            var policies = await policyRepository.GetLatestAsync();
            
            
              
            // Assert 
            var policiesInOrder = policies.OrderBy(p => p.Name).ToArray();
            
            Assert.Equal(2, policiesInOrder .Length);

            var file1Name = "file1";
            Assert.Equal(file1Name, policiesInOrder.First().Name);
            var file1Content = "{}";
            Assert.Equal(file1Content, policiesInOrder.First().Document);
            
            
            var file2Name = "file2";
            Assert.Equal(file2Name, policiesInOrder.Last().Name);
            var file2Content = @"{ ""Version"": ""2012-10-17"" }";
            Assert.Equal(file2Content, policiesInOrder.Last().Document);
        }
    }
}