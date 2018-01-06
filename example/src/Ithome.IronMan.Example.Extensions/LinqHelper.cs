using System;
using System.Collections.Generic;

namespace System.Linq
{
    internal static class LinqHelper 
    {
        internal static TSeed Aggregate<T,TSeed>(this IEnumerable<T> seq,TSeed seed,Action<TSeed,T> action)
            => seq.Aggregate(
                seed,
                (left,right) =>{
                    action(left,right);
                    return left;
                });
    }
}
