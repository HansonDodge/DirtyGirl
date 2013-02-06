using System;
using System.Collections.Generic;
using System.Linq;

namespace DirtyGirl.Web.Helpers
{
    public static class DistinctByExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource>(this IEnumerable<TSource> items, Func<TSource, TSource, bool> equalityComparer) where TSource : class
        {
            return items.Distinct(new LambdaComparer<TSource>(equalityComparer));
        } 
    }
}