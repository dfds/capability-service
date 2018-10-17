﻿using System;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Controllers;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using Moq;
using Xunit;

namespace DFDS.TeamService.Tests
{
    public class TestAwsConsoleController
    {
        [Fact]
        public async Task get_console_link_returns_url()
        {
            var consoleBuilder = new Moq.Mock<IAwsConsoleLinkBuilder>();

            consoleBuilder.Setup(c => c.GenerateUriForConsole(It.IsAny<string>()))
                .Returns(Task.FromResult(new Uri("http://bogus")));

            var sut = new AwsConsoleController(consoleBuilder.Object);
            var tokenId = "myFancyToken";
            var result = await sut.ConstructLink(tokenId);

            Assert.NotNull(result.Value.AbsoluteUrl);
        }
    }
}