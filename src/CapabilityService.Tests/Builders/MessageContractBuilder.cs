using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Models;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class MessageContractBuilder
    {
        private string _type;
        private string _content;
        private string _description;

        public MessageContractBuilder()
        {
            _type = "foo-type";
            _content = "foo-content";
            _description = "foo-description";
        }

        public MessageContractBuilder WithType(string type)
        {
            _type = type;
            return this;
        }

        public MessageContractBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public MessageContractBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public MessageContract Build()
        {
            return new MessageContract(_type, _description, _content);
        }
    }
}