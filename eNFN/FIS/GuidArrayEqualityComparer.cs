using System;
using System.Collections.Generic;
using System.Linq;

namespace eNFN.eANFIS.Impl
{
    internal class GuidArrayEqualityComparer : IEqualityComparer<IReadOnlyList<Guid>>
    {
        public bool Equals(IReadOnlyList<Guid> x, IReadOnlyList<Guid> y)
        {
            if (x == null) 
                throw new ArgumentNullException(nameof(x));
            if (y == null) 
                throw new ArgumentNullException(nameof(y));
            
            if (x.Count != y.Count)
            {
                return false;
            }

            return !x.Where((t, i) => t != y[i]).Any();
        }

        public int GetHashCode(IReadOnlyList<Guid> obj)
        {
            var result = 17;
            foreach (var t in obj)
            {
                unchecked
                {
                    result = result * 23 + t.GetHashCode();
                }
            }

            return result;
        }
    }
}