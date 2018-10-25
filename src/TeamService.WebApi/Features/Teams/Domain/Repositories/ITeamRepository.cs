using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAll();
        Task<IEnumerable<Team>> GetByUserId(string userId);

        Task<Team> GetById(Guid id);

        Task Add(Team team);
        Task<bool> Exists(Guid id);
        Task<bool> ExistsWithName(string name);
    }
}