using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Capability : AggregateRoot<Guid>
    {
        private static readonly string ROOTID_SALT = "fvvjaaqpagbb";
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
            
            RaiseEvent(new CapabilityCreated(
                capabilityId: Id,
                capabilityName: Name
            ));
        }

        // Used by Entity Framework to construct a object
        private Capability()
        {           
        }

        public void Delete()
        {
            RaiseEvent(new CapabilityDeleted(
                capabilityId: Id,
                capabilityName: Name
            ));
        }
        
        public void UpdateInfoFields(string newName, string newDescription)
        {
            Name = newName;
            Description = newDescription;
            
            RaiseEvent(new CapabilityUpdated(
                capabilityId: Id,
                capabilityName: Name,
                capabilityDescription: Description
            ));
        }

        private static string GenerateRootId(string name, Guid id)
        {
            const int maxPreservedNameLength = 22;
            
            if (name.Length < 2)
                throw new ArgumentException("Value is too short", nameof(name));

            var microHash = new HashidsNet.Hashids(ROOTID_SALT, 5, "abcdefghijklmnopqrstuvwxyz").EncodeHex(id.ToString("N")).Substring(0,5);
            
            var rootId = (name.Length > maxPreservedNameLength)
                ? $"{name.Substring(0, maxPreservedNameLength)}-{microHash}"
                : $"{name}-{microHash}";
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

        public Topic AddTopic(string name, string description, bool isPrivate = false)
        {
            return Topic.Create(name, description, isPrivate, Id);
        }
    }
}