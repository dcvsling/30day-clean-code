using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{

    internal static class LinqExtensions
    {
        public static void Each<T>(this IEnumerable<T> seq, Action<T> action)
        {
            foreach (var t in seq)
            {
                action(t);
            }
        }
    }
}
