using System;
using System.Collections.Generic;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class MemberEqualityComparer : IEqualityComparer<Member>
    {
        public bool Equals(Member x, Member y)
        {
            return x.Email.Equals(y.Email, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Member obj)
        {
            return obj.Email.GetHashCode();
        }
    }
}