using System;

namespace DFDS.TeamService.WebApi.Features.Teams.Domain.Events
{
    public class TeamCreated : IDomainEvent
    {
        public TeamCreated(Guid teamId, string teamName, string department)
        {
            TeamId = teamId;
            TeamName = teamName;
            Department = department;
        }

        public Guid TeamId { get; private set; }
        public string TeamName { get; private set; }
        public string Department { get; private set; }
    }
}