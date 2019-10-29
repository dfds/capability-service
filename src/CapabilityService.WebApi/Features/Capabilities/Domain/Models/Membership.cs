using System;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models
{
    public class Membership
    {
        private Membership()
        {
                
        }

        public Membership(Guid id, Member member)
        {
            Id = id;
            Member = member;
        }

        public Guid Id { get; private set; }
        public Member Member { get; private set; }

        public static Membership StartFor(Member member)
        {
            return new Membership(
                id: Guid.NewGuid(),
                member: member
            );
        }
    }
}