using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Shared
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> source, int nSize = 30)
        {
            var count = source.Count();
            for (int i = 0; i < count; i += nSize)
            {
                //yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
                yield return source.SubEnumerable(i, Math.Min(nSize, count - i));
            }
        }
    }
}
