using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using DFDS.TeamService.WebApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Persistence
{
    public class DbUserRepository : IUserRepository
    {
        private readonly TeamServiceDbContext _dbContext;

        public DbUserRepository(TeamServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetById(string userId)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);
        }

        public async Task Add(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }
    }

    public class CognitoDecorator : IUserRepository
    {
        private readonly IUserRepository _inner;
        private readonly CognitoUserProvider _userProvider;

        public CognitoDecorator(IUserRepository inner, CognitoUserProvider userProvider)
        {
            _inner = inner;
            _userProvider = userProvider;
        }

        public Task Add(User user) => _inner.Add(user);

        public async Task<User> GetById(string userId)
        {
            var user = await _inner.GetById(userId);
            if (user != null)
            {
                return user;
            }

            user = await _userProvider.GetUserById(userId);
            if (user != null)
            {
                await _inner.Add(user);
            }

            return user;
        }
    }

    public class CognitoUserProvider
    {
        private readonly AmazonCognitoIdentityProviderClient _identityProviderClient;
        private readonly string _userPoolId;

        public CognitoUserProvider(AWSCredentials awsCredentials, string userPoolId)
        {
            _identityProviderClient = new AmazonCognitoIdentityProviderClient(awsCredentials, RegionEndpoint.EUCentral1);
            _userPoolId = userPoolId;
        }

        public async Task<User> GetUserById(string id)
        {
            try
            {
                var request = new AdminGetUserRequest
                {
                    Username = id,
                    UserPoolId = _userPoolId
                };

                var response = await _identityProviderClient.AdminGetUserAsync(request);

                var nameAttribute = response.UserAttributes.FirstOrDefault(x => x.Name == "name");
                var emailAttribute = response.UserAttributes.FirstOrDefault(x => x.Name == "email");

                return new User(
                    id: id,
                    name: nameAttribute?.Value,
                    email: emailAttribute?.Value
                );
            }
            catch (UserNotFoundException)
            {
                return null;
            }
        }
    }
}