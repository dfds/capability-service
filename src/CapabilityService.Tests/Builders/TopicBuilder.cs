using System;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class TopicBuilder
    {
        private Guid _id;
        private string _name;
        private string _description;
        private bool _isPrivate;
        private Guid _capabilityId;

        public TopicBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = "foo";
            _description = "bar";
            _isPrivate = false;
            _capabilityId = new Guid("11111111-1111-1111-1111-111111111111");
        }

        public Topic Build()
        {
            return new Topic(_id, _name, _description, _isPrivate, _capabilityId);
        }
    }
}