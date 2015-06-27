using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wedblob.Web.Infrastructure.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> GroupsOf<T>(this IEnumerable<T> enumerable, int groupSize){

            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Take(2);
            }
        }

        public static IEnumerable<T> Take<T>(this IEnumerator<T> enumerator, int count)
        {
            yield return enumerator.Current;
            for (var i = 1; i < count && enumerator.MoveNext(); i++)
                yield return enumerator.Current;
        }
    }

    

}