using System;
using System.Net;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using Moq;
using Xunit;

namespace DFDS.TeamService.Tests.Features.UserServices
{
    public class UserServiceRoutesFacts
    {
        [Fact]
        public async Task GIVEN_none_existing_userId_EXPECT_NotFound()
        {
            using (var builder = new HttpClientBuilder())
            {
                // Arrange
                var userRepositoryBuilder = new Mock<IUserRepository>();
                userRepositoryBuilder
                    .Setup(u => u.GetById(It.IsAny<string>()))
                    .ReturnsAsync((User)null);
                
                
                var client = builder
                    .WithService(userRepositoryBuilder.Object)
                    .Build();


                // Act
                var userIdThatDoesNotExist = "userIdThatDoesNotExist";
                var response = await client.GetAsync($"api/users/{userIdThatDoesNotExist}/services");
            
                
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    response.StatusCode
                );
            }
        }

        
        [Fact]
        public async Task GIVEN_uerId_EXPECT_Ok()
        {
            using (var builder = new HttpClientBuilder())
            {
                // Arrange
                var userId = "user1";
                var user = new User(
                    id: userId, 
                    name: "morten", 
                    email: "morten47@hotmail.com"
                );
                
                var userRepositoryBuilder = new Mock<IUserRepository>();
                userRepositoryBuilder
                    .Setup(u => u.GetById(It.Is<string>(s => s.Equals(userId))))
                    .ReturnsAsync(user);
                
                
                var client = builder
                    .WithService(userRepositoryBuilder.Object)
                    .Build();


                // Act
                var response = await client.GetAsync($"api/users/{userId}/services");
            
                
                // Assert
                Assert.Equal(
                    HttpStatusCode.OK,
                    response.StatusCode
                );
            } 
        }
    }
}