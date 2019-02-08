using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.CapabilityService.WebApi.DomainEvents;

namespace DFDS.CapabilityService.WebApi.Models
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

    public class CapabilityCreated : DomainEvent
    {
        public CapabilityCreated(Guid capabilityId, string capabilityName)
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
        }

        public Guid CapabilityId { get; }
        public string CapabilityName { get; }
    }

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

    public class MemberEqualityComparer : IEqualityComparer<Member>
    {
        public bool Equals(Member x, Member y)
        {
            return x.Email.Equals(y.Email, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Member obj)
        {
            return obj.Email.GetHashCode();
        }
    }

    public class Member
    {
        private Member()
        {
            
        }

        public Member(string email)
        {
            Email = email;
        }

        public string Email { get; private set; }
    }
}