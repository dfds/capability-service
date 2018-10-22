using System;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Controllers;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using Moq;
using Xunit;

namespace DFDS.TeamService.Tests
{
    public class TestAwsConsoleUrlController
    {
        [Fact]
        public async Task get_console_link_returns_url()
        {
            var consoleBuilder = new Moq.Mock<IAwsConsoleUrlBuilder>();

            consoleBuilder
                .Setup(c => c.GenerateUriForConsole(
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    )
                )
                .Returns(Task.FromResult(new Uri("http://bogus")));

            var sut = new AwsConsoleUrlController(consoleBuilder.Object, new TeamNameToRoleNameConverter());
            var tokenId = "myFancyToken";
            var teamName = "aGreatTeam";
            var result = await sut.GetConsoleUrl(tokenId, teamName);

            Assert.NotNull(result.Value.AbsoluteUrl);
        }
    }
}