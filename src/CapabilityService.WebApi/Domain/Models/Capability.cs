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
        private readonly List<Context> _contexts = new List<Context>();
  

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string RootId { get; private set; }
        public IEnumerable<Member> Members => _memberships.Select(x => x.Member).Distinct(new MemberEqualityComparer());
        public IEnumerable<Membership> Memberships => _memberships;
        public IEnumerable<Context> Contexts => _contexts;
        
        
        public Capability(Guid id, string name, string rootId, string description, IEnumerable<Membership> memberships, IEnumerable<Context> contexts)
        {
            Id = id;
            Name = name;
            RootId = rootId;
            Description = description;
            _memberships.AddRange(memberships);
            _contexts.AddRange(contexts);
        }

        private Capability()
        {
            
        }

        
        public static Capability Create(string name, string description)
        {
            var capability = new Capability(
                id: Guid.NewGuid(),
                name: name,
                rootId: GenerateRootId(name),
                description: description,
                memberships: Enumerable.Empty<Membership>(),
                contexts:Enumerable.Empty<Context>()
            );

            capability.RaiseEvent(new CapabilityCreated(
                capabilityId: capability.Id,
                capabilityName: capability.Name
            ));

            return capability;
        }

        private static string GenerateRootId(string name)
        {
            if (name.Length < 2)
                throw new ArgumentException("Value is too short", nameof(name));
            
            var guidBase = Guid.NewGuid().ToString().Substring(0, 7);
            var rootId = (name.Length > 20)
                ? $"{name.Substring(0, 20)}-{guidBase}"
                : $"{name}-{guidBase}";
            return rootId.ToLowerInvariant();
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
            
            RaiseEvent(new ContextAddedToCapability(
                Id, 
                Name,
                RootId,
                context.Id,
                contextName
            ));
        }

        public void UpdateContext(Guid contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail)
        {
            var context = Contexts.FirstOrDefault(c => c.Id == contextId);
            if(context == null)
                throw new ContextDoesNotExistException();
            
            context.UpdateContext(awsAccountId, awsRoleArn, awsRoleEmail);
            RaiseEvent(new ContextUpdated(Id, contextId, awsAccountId, awsRoleArn, awsRoleEmail ));

        }
    }
}