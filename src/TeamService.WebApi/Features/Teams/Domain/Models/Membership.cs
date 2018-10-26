using System;

namespace DFDS.TeamService.WebApi.Features.Teams.Domain.Models
{
    public class Membership : Entity<Guid>
    {
        private Membership()
        {
            
        }

        public Membership(Guid id, User user, MembershipType type, DateTime startedDate) : base(id)
        {
            User = user;
            Type = type;
            StartedDate = startedDate;
        }

        public User User { get; private set; }
        public MembershipType Type { get; private set; }
        public DateTime StartedDate { get; private set; }

        public static Membership Start(User user, MembershipType type)
        {
            return new Membership(
                id: Guid.NewGuid(),
                user: user,
                type: type,
                startedDate: DateTime.UtcNow
            );
        }
    }
}