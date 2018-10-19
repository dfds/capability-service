using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.TeamService.WebApi.Features.Teams.Application
{
    public class TeamApplicationServiceTransactionDecorator : ITeamApplicationService
    {
        private readonly ITeamApplicationService _inner;
        private readonly UnitOfWork _unitOfWork;

        public TeamApplicationServiceTransactionDecorator(ITeamApplicationService inner, TeamServiceDbContext dbContext)
        {
            _inner = inner;
            _unitOfWork = new UnitOfWork(dbContext);
        }

        public Task<IEnumerable<Team>> GetAllTeams()
        {
            return _inner.GetAllTeams();
        }

        public Task<Team> GetTeam(Guid id)
        {
            return _inner.GetTeam(id);
        }

        public async Task<Team> CreateTeam(string name, string department)
        {
            return await _unitOfWork.Run(() => _inner.CreateTeam(name, department));
        }

        public Task<bool> Exists(string teamName)
        {
            return _inner.Exists(teamName);
        }

        public Task JoinTeam(Guid teamId, string userId)
        {
            return _unitOfWork.Run(() => _inner.JoinTeam(teamId, userId));
        }
    }

    public class UnitOfWork
    {
        private readonly DbContext _dbContext;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Run(Func<Task> action)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                await action();

                _dbContext.SaveChanges();
                transaction.Commit();
            }
        }

        public async Task<TResult> Run<TResult>(Func<Task<TResult>> action)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var result = await action();

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return result;
            }
        }
    }
}