using System;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class CapabilityBuilder
    {
        private Guid _id;
        private string _name;
        private Membership[] _memberships;

        public CapabilityBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = "foo";
            _memberships = new Membership[0];
        }

        public CapabilityBuilder WithMembers(params string[] members)
        {
            _memberships = members
                       .Select(email => Membership.StartFor(new Member(email)))
                       .ToArray();

            return this;
        }

        public Capability Build()
        {
            return new Capability(_id, _name, _memberships);
        }
    }
}