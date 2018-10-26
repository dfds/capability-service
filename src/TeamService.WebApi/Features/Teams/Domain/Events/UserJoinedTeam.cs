using System;
using DFDS.TeamService.WebApi.DomainEvents;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.WebApi.Features.Teams.Domain.Events
{
    public class UserJoinedTeam : DomainEvent
    {
        public UserJoinedTeam(Guid teamId, string userId, MembershipType userHasRole, DateTime startedDate)
        {
            TeamId = teamId;
            UserId = userId;
            UserHasRole = userHasRole;
            StartedDate = startedDate;
        }

        public Guid TeamId { get; private set; }
        public string UserId { get; private set; }
        public MembershipType UserHasRole { get; private set; }
        public DateTime StartedDate { get; private set; }
    }
}