using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DFDS.TeamService.WebApi
{
    public class PropertyEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> _selector;

        public PropertyEqualityComparer(Expression<Func<T, object>> selector)
        {
            _selector = selector.Compile();
        }

        public bool Equals(T x, T y)
        {
            var left = _selector(x);
            var right = _selector(y);

            return left.Equals(right);
        }

        public int GetHashCode(T obj)
        {
            return _selector(obj).GetHashCode();
        }
    }
}