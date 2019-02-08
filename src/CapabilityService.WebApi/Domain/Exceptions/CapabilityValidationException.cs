using System;

namespace DFDS.CapabilityService.WebApi.Domain.Exceptions
{
    public class CapabilityValidationException : ArgumentException
    {
        public CapabilityValidationException(string message) : base(message)
        {
        }

        public CapabilityValidationException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}