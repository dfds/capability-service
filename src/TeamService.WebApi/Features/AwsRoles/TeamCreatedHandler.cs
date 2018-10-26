using System.Threading.Tasks;
using DFDS.TeamService.WebApi.DomainEvents;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Events;

namespace DFDS.TeamService.WebApi.Features.AwsRoles
{
    public class TeamCreatedHandler : IDomainEventSubscriber<TeamCreated>
    {
        private readonly IAwsIdentityClient _identityClient;

        public TeamCreatedHandler(IAwsIdentityClient identityClient)
        {
            _identityClient = identityClient;
        }

        public async Task Handle(TeamCreated domainEvent)
        {
            await _identityClient.PutRoleAsync(domainEvent.TeamName);
        }
    }
}