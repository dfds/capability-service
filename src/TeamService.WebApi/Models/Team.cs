using System;
using System.Collections.Generic;
using System.Linq;

namespace DFDS.TeamService.WebApi.Models
{
    public class Team
    {
        private readonly List<Membership> _memberships = new List<Membership>();

        private Team()
        {
            
        }

        public Team(Guid id, string name, IEnumerable<Membership> memberships)
        {
            Id = id;
            Name = name;
            _memberships.AddRange(memberships);
        }

        public Guid Id { get; private set; }
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
                throw new NotMemberOfTeamException();
            }

            _memberships.Remove(membership);
        }

        public static Team Create(string name)
        {
            return new Team(
                id: Guid.NewGuid(),
                name: name,
                memberships: Enumerable.Empty<Membership>()
            );
        }
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