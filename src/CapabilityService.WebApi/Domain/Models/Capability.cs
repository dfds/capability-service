using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Capability : AggregateRoot<Guid>
    {
        private readonly List<Membership> _memberships = new List<Membership>();
        private readonly List<Context> _contexts = new List<Context>();
  

        public string Name { get; private set; }
        public IEnumerable<Member> Members => _memberships.Select(x => x.Member).Distinct(new MemberEqualityComparer());
        public IEnumerable<Membership> Memberships => _memberships;
        public IEnumerable<Context> Contexts => _contexts;
        
        
        public Capability(Guid id, string name, IEnumerable<Membership> memberships, IEnumerable<Context> contexts)
        {
            Id = id;
            Name = name;
            _memberships.AddRange(memberships);
            _contexts.AddRange(contexts);
        }

        private Capability()
        {
            
        }

        
        public static Capability Create(string name)
        {
            var capability = new Capability(
                id: Guid.NewGuid(),
                name: name,
                memberships: Enumerable.Empty<Membership>(),
                contexts:Enumerable.Empty<Context>()
            );

            capability.RaiseEvent(new CapabilityCreated(
                capabilityId: capability.Id,
                capabilityName: capability.Name
            ));

            return capability;
        }
        
        
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
            RaiseEvent(new MemberLeftCapability(Id, memberEmail));
        }

        
        private bool ContextExists(string contextName)
        {
            return Contexts.Any(c => c.Name.Equals(contextName, StringComparison.InvariantCultureIgnoreCase));
        }
        
        public void AddContext(string contextName)
        {
            if (ContextExists(contextName))
            {
                return;
            }
            
            var context = new Context(Guid.NewGuid(), contextName);
            
            _contexts.Add(context);
            
            RaiseEvent(new ContextAddedToCapability(Id, context.Id,contextName));
        }
        
 
    }
}