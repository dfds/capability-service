using System;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class MessageContract : IEquatable<MessageContract>
    {
        public MessageContract(string type, string description, string content)
        {
            Type = type;
            Description = description;
            Content = content;
        }

        public string Type { get; private set; }
        public string Content { get; set; }
        public string Description { get; set; }

        public bool Equals(MessageContract other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((MessageContract) obj);
        }

        public override int GetHashCode()
        {
            return (Type != null
                ? Type.GetHashCode()
                : 0);
        }

        public static bool operator ==(MessageContract left, MessageContract right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MessageContract left, MessageContract right)
        {
            return !Equals(left, right);
        }
    }
}