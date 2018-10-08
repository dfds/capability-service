using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Clients;
using Shouldly;
using Xunit;

namespace DFDS.TeamService.IntegrationTests
{
    /// <summary>
    /// Method names should be read as "User pool client can {method name}"
    /// </summary>
    public class UserPoolClientFacts
    {
        [Fact]
        public async Task CreateGroup()
        {
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());


            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var groupName = CreateName();
                await userPoolClient.CreateGroupAsync(groupName);
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }

        [Fact]
        public async Task CreateUser()
        {
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var userName = CreateName();
                await userPoolClient.CreateUserAsync(userName);
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }

        [Fact]
        public async Task GetUserAttributesForAUserThatDoesNotExist()
        {
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var userName = CreateName();
                var result = await userPoolClient.GetUserAttributesAsync(userName);

                result.Handle(
                    success => throw new Exception("This function should not succeed"),
                    notFound => { notFound.Message.ShouldBe($"the user '{userName}' does not exist"); }
                );
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }

        
        [Fact]
        public async Task GetUserAttributesForAUser()
        {
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var userName = CreateName();
                await userPoolClient.CreateUserAsync(userName);
                
                
                var result = await userPoolClient.GetUserAttributesAsync(userName);

                result.Handle(
                    success => { },
                    notFound =>
                        {
                            throw new Exception("This function should not succeed");
                        }
                    );
                        
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }
        
        
        [Fact]
        public async Task AddUserToGroup()
        {
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var userName = CreateName();
                await userPoolClient.CreateUserAsync(userName);

                var groupName = CreateName();
                await userPoolClient.CreateGroupAsync(groupName);

                await userPoolClient.AddUserToGroup(userName, groupName);
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }


        [Fact]
        public async Task AddUserToGroupThatDoesNotExist()
        {
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var userName = CreateName();
                await userPoolClient.CreateUserAsync(userName);

                var groupName = CreateName();

                await userPoolClient.AddUserToGroup(userName, groupName);
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }

        [Fact]
        public async Task AddUserToGroupTwice()
        {
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var userName = CreateName();
                await userPoolClient.CreateUserAsync(userName);

                var groupName = CreateName();
                await userPoolClient.CreateGroupAsync(groupName);

                await userPoolClient.AddUserToGroup(userName, groupName);
                await userPoolClient.AddUserToGroup(userName, groupName);
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }

        [Fact]
        public async Task ListSixtyOneUsersInAGroup()
        {
            // Arrange
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);


                var groupName = CreateName();
                await userPoolClient.CreateGroupAsync(groupName);
                var users = new List<string>();
                for (var i = 0; i < 26; i++)
                {
                    var userName = CreateName();

                    users.Add(userName);

                    await userPoolClient.CreateUserAsync(userName);
                    await userPoolClient.AddUserToGroup(userName, groupName);
                }

                // Act
                var usersInGroup = await userPoolClient.ListUsersInGroupAsync(groupName);


                // Assert
                usersInGroup
                    .Select(u => u.Username)
                    .OrderBy(u => u)
                    .ToList()
                    .ShouldBe(users.OrderBy(g => g));
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }

        [Fact]
        public async Task ListOneGroup()
        {
            // Arrange
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var groupName = CreateName();
                await userPoolClient.CreateGroupAsync(groupName);

                // Act
                var groupsResult = await userPoolClient.ListGroupsAsync();

                // Assert
                Assert.Equal(groupName, groupsResult.Single());
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }


        [Fact]
        public async Task ListTwentyFiveGroups()
        {
            // Arrange
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);

                var groups = new List<string>();
                for (var i = 0; i < 25; i++)
                {
                    var groupName = CreateName();
                    await userPoolClient.CreateGroupAsync(groupName);

                    groups.Add(groupName);
                }

                // Act
                var groupsResult = await userPoolClient.ListGroupsAsync();

                // Assert
                groupsResult.OrderBy(g => g).ShouldBe(groups.OrderBy(g => g));
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }


        [Fact]
        public async Task GetAGroupThatDoesNotExist()
        {
            // Arrange
            var client = CognitoClientFactory.CreateFromGetEnvironmentVariables();
            var userPollId = await client.CreateUserPoolAsync(CreateName());

            try
            {
                var userPoolClient = CreateUserPoolClient(userPollId);
                var groupName = CreateName();
                var group = await userPoolClient.GetGroupAsync(groupName);

                group.ShouldBeNull();
            }
            finally
            {
                await client.DeleteUserPoolAsync(userPollId);
            }
        }


        public string CreateName()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-THH-mm") + "_" + Guid.NewGuid().ToString().Substring(0, 8);
        }

        public UserPoolClient CreateUserPoolClient(string userPoolId)
        {
            var accessKey = Environment.GetEnvironmentVariable("AWS_accessKey");
            var secretKey = Environment.GetEnvironmentVariable("AWS_secretKey");

            var userPoolClient = new UserPoolClient(
                accessKey,
                secretKey,
                userPoolId
            );


            return userPoolClient;
        }
    }
}