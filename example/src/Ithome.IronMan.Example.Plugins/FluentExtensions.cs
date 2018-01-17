using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{

    public static class FluentExtensions
    {
        public static IChain<TNext> Then<T, TNext>(this IChain<T> chain, Func<T, TNext> func)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, func));

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChain<T> chain, Func<T, Task<TNext>> next)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, next));

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChainAwaiter<T> chain, Func<T, Task<TNext>> next)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, next));

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChainAwaiter<T> chain, Func<T, TNext> next)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, next));
    }
}
