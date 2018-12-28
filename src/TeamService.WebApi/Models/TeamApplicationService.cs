using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            var response = await _client.PostAsJsonAsync("/api/roles", new {Name = teamName});
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<RoleInformation>();

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
            var response = await _client.PostAsJsonAsync("/api/roles", new {RoleName = teamName, RoleArn = roleIdentifier});
            response.EnsureSuccessStatusCode();
        }
    }
}