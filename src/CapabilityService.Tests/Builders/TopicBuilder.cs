using System;
using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Models;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class TopicBuilder
    {
        private Guid _id;
        private string _name;
        private string _description;
        private bool _isPrivate;
        private Guid _capabilityId;

        private List<MessageContract> _messageContracts = new List<MessageContract>();

        public TopicBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = "foo";
            _description = "bar";
            _isPrivate = false;
            _capabilityId = new Guid("11111111-1111-1111-1111-111111111111");
        }

        public TopicBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public TopicBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public TopicBuilder WithMessageContracts(params MessageContract[] messageContracts)
        {
            _messageContracts = new List<MessageContract>(messageContracts);
            return this;
        }

        public Topic Build()
        {
            return new Topic(_id, _name, _description, _isPrivate, _capabilityId, _messageContracts);
        }
    }
}