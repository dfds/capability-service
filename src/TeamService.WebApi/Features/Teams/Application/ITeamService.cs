using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.WebApi.Features.Teams.Application
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllTeams();
        Task<Team> GetTeam(Guid id);

        Task<Team> CreateTeam(string name, string department);
        Task<bool> Exists(string teamName);
        Task JoinTeam(Guid teamId, string userId);
    }
}