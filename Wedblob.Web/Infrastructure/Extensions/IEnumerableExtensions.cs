using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wedblob.Web.Infrastructure.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> GroupsOf<T>(this IEnumerable<T> source, int groupSize)
        {
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Take(2);
            }
        }

        public static IEnumerable<T> Take<T>(this IEnumerator<T> source, int count)
        {
            yield return source.Current;
            for (var i = 1; i < count && source.MoveNext(); i++)
                yield return source.Current;
        }

        public static IEnumerable<string> OfToStrings(this IEnumerable source){
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current == null)
                    continue;

                yield  return current.ToString();
            }
        }
    }

    

}