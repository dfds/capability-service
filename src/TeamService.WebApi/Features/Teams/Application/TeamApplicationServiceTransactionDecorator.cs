using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Persistence;

namespace DFDS.TeamService.WebApi.Features.Teams.Application
{
    public class TeamApplicationServiceTransactionDecorator : ITeamService
    {
        private readonly ITeamService _inner;
        private readonly TeamServiceDbContext _dbContext;

        public TeamApplicationServiceTransactionDecorator(ITeamService inner, TeamServiceDbContext dbContext)
        {
            _inner = inner;
            _dbContext = dbContext;
        }

        public Task<IEnumerable<Team>> GetAllTeams()
        {
            return _inner.GetAllTeams();
        }

        public Task<Team> GetTeam(Guid id)
        {
            return _inner.GetTeam(id);
        }

        private async Task WrapInTransaction(Func<Task> action)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                await action();

                _dbContext.SaveChanges();
                transaction.Commit();
            }
        }

        private async Task<T> WrapInTransaction<T>(Func<Task<T>> action)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var result = await action();

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return result;
            }
        }

        public async Task<Team> CreateTeam(string name, string department)
        {
            return await WrapInTransaction(() => _inner.CreateTeam(name, department));
        }

        public Task<bool> Exists(string teamName)
        {
            return _inner.Exists(teamName);
        }

        public Task JoinTeam(Guid teamId, string userId)
        {
            return WrapInTransaction(() => _inner.JoinTeam(teamId, userId));
        }
    }
}