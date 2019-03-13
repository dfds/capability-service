using System.Collections.Generic;
using System.Linq;

namespace DFDS.CapabilityService.Tests.Helpers
{
    public class PropertiesComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            var properties = x.GetType()
                              .GetProperties()
                              .ToArray();
            
            return properties.All(p => object.Equals(p.GetValue(x), p.GetValue(y)));
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}