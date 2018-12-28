using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<Team> CreateTeam(string name)
        {
            var team = Team.Create(name);
            await _teamRepository.Add(team);
            await _roleService.CreateRoleFor(team);

            return team;
        }

        public Task<Team> GetTeam(Guid id) => _teamRepository.Get(id);
        public Task<IEnumerable<Team>> GetAllTeams() => _teamRepository.GetAll();
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