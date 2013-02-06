using System;
using System.Collections.Generic;
using System.Linq;

namespace DirtyGirl.Web.Helpers
{
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _lambdaComparer;
        private readonly Func<T, int> _lambdaHash;

        public LambdaComparer(Func<T, T, bool> lambdaComparer) :
            this(lambdaComparer, o => o.GetHashCode())
        {
        }

        public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
        {
            if (lambdaComparer == null)
                throw new ArgumentNullException("lambdaComparer");
            if (lambdaHash == null)
                throw new ArgumentNullException("lambdaHash");
            this._lambdaComparer = lambdaComparer;
            this._lambdaHash = lambdaHash;
        }

        public Func<T, int> LambdaHash
        {
            get { return _lambdaHash; }
        }

        public bool Equals(T x, T y)
        {
            return _lambdaComparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }

    public static class Ext
    {
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first,
                                                           IEnumerable<TSource> second,
                                                           Func<TSource, TSource, bool> comparer)
        {
            return first.Except(second, new LambdaComparer<TSource>(comparer));
        }
    }
}
