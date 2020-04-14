using System;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class CapabilityBuilder
    {
        private Guid _id;
        private CapabilityName _name;
        private string _rootId;
        private string _description;
        private Membership[] _memberships;
        private Context[] _contexts;
        public CapabilityBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = new CapabilityName("Foo");
            _rootId = "foo-582a4";
            _description = "bar";
            _memberships = new Membership[0];
            _contexts = new Context[0];
        }

        public CapabilityBuilder WithMembers(params string[] members)
        {
            _memberships = members
                       .Select(email => Membership.StartFor(new Member(email)))
                       .ToArray();

            return this;
        }
        
        public CapabilityBuilder WithContexts(params Context[] contexts)
        {
            _contexts = contexts;
            return this;
        }

        public CapabilityBuilder WithRootId(string rootId)
        {
            _rootId = rootId;
            return this;
        }
        
        
        public Capability Build()
        {
            return new Capability(_id, _name, _rootId,_description, _memberships, _contexts);
        }

    }
}
