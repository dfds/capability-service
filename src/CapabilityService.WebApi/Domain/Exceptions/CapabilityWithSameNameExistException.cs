using System;

namespace DFDS.CapabilityService.WebApi.Domain.Exceptions
{
    public class CapabilityWithSameNameExistException : Exception
    {
        public CapabilityWithSameNameExistException(string name): base($"A capability with the name:'{name}' already exits, please give your capability a other name."){}
	    
    }
}
