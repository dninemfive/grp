using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace grp
{    
    /// <summary>
    /// Contains various utility extensions.
    /// </summary>
    public static class Extensions
    {                
        public static IEnumerable<IEnumerable<T>> BreakInto<T>(this IEnumerable<T> original, int parts)
        {
            int partSize = original.Count() / parts;
            int remainder = original.Count() - (parts * partSize);
            int ct = 0;
            for(int i = 0; i < parts; i++)
            {
                int thisSize = partSize + (remainder-- > 0 ? 1 : 0);                
                int endSize = original.Count() - thisSize - ct;
                yield return original.Skip(ct).SkipLast(endSize);
                ct += thisSize;
            }           
        }
    }
}
