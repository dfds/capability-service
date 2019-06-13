namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class StringSubstitutable
    {
        private readonly string _value;

        protected StringSubstitutable(string value)
        {
            _value = value;
        }
        
        public static implicit operator string(StringSubstitutable stringSubstitutable)
        {
            return stringSubstitutable?._value;
        }
        
        public override string ToString()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType() && obj.GetType() != typeof(string))
            { return false;}
            
            switch (obj)
            {
                case StringSubstitutable stringSubstitutable:
                    return _value.Equals(stringSubstitutable._value);
                case string item:
                    return _value.Equals(item);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}