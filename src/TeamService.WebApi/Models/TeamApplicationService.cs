using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.TeamService.WebApi.Models
{
    public class TeamApplicationService : ITeamApplicationService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IRoleService _roleService;

        public TeamApplicationService(ITeamRepository teamRepository, IRoleService roleService)
        {
            _teamRepository = teamRepository;
            _roleService = roleService;
        }

        public Task<Team> GetTeam(Guid id) => _teamRepository.Get(id);
        public Task<IEnumerable<Team>> GetAllTeams() => _teamRepository.GetAll();

        public async Task<Team> CreateTeam(string name)
        {
            var team = Team.Create(name);
            await _teamRepository.Add(team);
            await _roleService.CreateRoleFor(team);

            return team;
        }

        public async Task JoinTeam(Guid teamId, string memberEmail)
        {
            var team = await _teamRepository.Get(teamId);
            team.AcceptNewMember(memberEmail);
        }
    }

    public class TeamTransactionalDecorator : ITeamApplicationService
    {
        private readonly ITeamApplicationService _inner;
        private readonly TeamServiceDbContext _dbContext;

        public TeamTransactionalDecorator(ITeamApplicationService inner, TeamServiceDbContext dbContext)
        {
            _inner = inner;
            _dbContext = dbContext;
        }

        public Task<IEnumerable<Team>> GetAllTeams() => _inner.GetAllTeams();
        public Task<Team> GetTeam(Guid id) => _inner.GetTeam(id);

        public async Task<Team> CreateTeam(string name)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var team = await _inner.CreateTeam(name);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return team;
            }
        }

        public async Task JoinTeam(Guid teamId, string memberEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.JoinTeam(teamId, memberEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }
    }

    public interface IRoleService
    {
        Task CreateRoleFor(Team team);
    }

    public class RoleService : IRoleService
    {
        private readonly IAMRoleServiceFacade _iamRoleService;
        private readonly RoleMapperServiceFacade _roleMapperService;

        public RoleService(IAMRoleServiceFacade iamRoleService, RoleMapperServiceFacade roleMapperService)
        {
            _iamRoleService = iamRoleService;
            _roleMapperService = roleMapperService;
        }
        
        public async Task CreateRoleFor(Team team)
        {
            var roleIdentifier = await _iamRoleService.CreateRole(team.Name);
            await _roleMapperService.CreateRoleMapping(team.Name, roleIdentifier);
        }
    }

    public class IAMRoleServiceFacade
    {
        private readonly HttpClient _client;

        public IAMRoleServiceFacade(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<string> CreateRole(string teamName)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            
            var requestPayload = new StringContent(
                content: JsonConvert.SerializeObject(new {Name = teamName}, serializerSettings),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );
            
            var response = await _client.PostAsync("/api/roles", requestPayload);
            response.EnsureSuccessStatusCode();

            var responsePayload = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RoleInformation>(responsePayload);

            return result.RoleArn;
        }

        private class RoleInformation
        {
            public string RoleArn { get; set; }
        }
    }

    public class RoleMapperServiceFacade
    {
        private readonly HttpClient _client;

        public RoleMapperServiceFacade(HttpClient client)
        {
            _client = client;
        }

        public async Task CreateRoleMapping(string teamName, string roleIdentifier)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var payload = new StringContent(
                content: JsonConvert.SerializeObject(new {RoleName = teamName, RoleArn = roleIdentifier}, serializerSettings),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );
            
            var response = await _client.PostAsync("/api/roles", payload);
            response.EnsureSuccessStatusCode();
        }
    }
}