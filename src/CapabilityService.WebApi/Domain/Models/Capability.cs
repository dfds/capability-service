using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Capability : AggregateRoot<Guid>
    {
        private readonly List<Membership> _memberships = new List<Membership>();

        private Capability()
        {
            
        }

        public Capability(Guid id, string name, IEnumerable<Membership> memberships)
        {
            Id = id;
            Name = name;
            _memberships.AddRange(memberships);
        }

        public string Name { get; private set; }
        public IEnumerable<Member> Members => _memberships.Select(x => x.Member).Distinct(new MemberEqualityComparer());
        public IEnumerable<Membership> Memberships => _memberships;

        private bool IsAlreadyMember(string memberEmail)
        {
            return Members.Any(member => member.Email.Equals(memberEmail, StringComparison.InvariantCultureIgnoreCase));
        }

        public void StartMembershipFor(string memberEmail)
        {
            if (IsAlreadyMember(memberEmail))
            {
                return;
            }

            var member = new Member(memberEmail);
            var membership = Membership.StartFor(member);
            _memberships.Add(membership);

            RaiseEvent(new MemberJoinedCapability(Id, memberEmail));
        }

        public void StopMembershipFor(string memberEmail)
        {
            var membership = _memberships.SingleOrDefault(x => x.Member.Email.Equals(memberEmail, StringComparison.InvariantCultureIgnoreCase));

            if (membership == null)
            {
                throw new NotMemberOfCapabilityException();
            }

            _memberships.Remove(membership);
        }

        public static Capability Create(string name)
        {
            var capability = new Capability(
                id: Guid.NewGuid(),
                name: name,
                memberships: Enumerable.Empty<Membership>()
            );

            capability.RaiseEvent(new CapabilityCreated(
                capabilityId: capability.Id,
                capabilityName: capability.Name
            ));

            return capability;
        }
    }

    public class MemberJoinedCapability : IDomainEvent
    {
        public MemberJoinedCapability(Guid capabilityId, string memberEmail)
        {
            CapabilityId = capabilityId;
            MemberEmail = memberEmail;
        }

        public Guid CapabilityId { get; private set; }
        public string MemberEmail { get; private set; }
    }

}