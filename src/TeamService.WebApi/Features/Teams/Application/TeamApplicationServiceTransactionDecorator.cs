using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Persistence;

namespace DFDS.TeamService.WebApi.Features.Teams.Application
{
    public class TeamApplicationServiceTransactionDecorator : ITeamApplicationService
    {
        private readonly ITeamApplicationService _inner;
        private readonly IUnitOfWork<TeamServiceDbContext> _unitOfWork;

        public TeamApplicationServiceTransactionDecorator(ITeamApplicationService inner, IUnitOfWork<TeamServiceDbContext> unitOfWork)
        {
            _inner = inner;
            _unitOfWork = unitOfWork;
        }

        public Task<Team> GetTeam(Guid id) => _inner.GetTeam(id);
        public Task<IEnumerable<Team>> GetAllTeams() => _inner.GetAllTeams();
        public Task<bool> Exists(string teamName) => _inner.Exists(teamName);

        public async Task<Team> CreateTeam(string name, string department)
        {
            return await _unitOfWork.Run(() => _inner.CreateTeam(name, department));
        }

        public Task JoinTeam(Guid teamId, string userId)
        {
            return _unitOfWork.Run(() => _inner.JoinTeam(teamId, userId));
        }
    }
}