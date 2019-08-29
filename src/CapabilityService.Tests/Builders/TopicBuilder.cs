using System;
using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class TopicBuilder
    {
        private Guid _id;
        private string _name;
        private string _nameBusinessArea;
        private string _nameType;
        private string _nameMisc;
        private string _description;
        private bool _isPrivate;
        private Guid _capabilityId;

        private List<MessageContract> _messageContracts = new List<MessageContract>();

        public TopicBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = "foo";
            _nameBusinessArea = "build";
            _nameType = "events";
            _nameMisc = "hello_pelle";
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
            return new Topic(_id, _name, _description, _nameBusinessArea, _nameType, _nameMisc, _isPrivate, _capabilityId, _messageContracts);
        }
    }
}