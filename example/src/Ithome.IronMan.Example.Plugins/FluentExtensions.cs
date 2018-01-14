using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{

    public static class FluentExtensions
    {
        public static IChain<TNext> Then<T, TNext>(this IChain<T> chain, Func<T, TNext> func)
            => chain is IChainFactoryAccessor accessor
                ? accessor.Factory.Create(() => func(chain.Result))
                : throw new Exception();

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChain<T> chain, Func<T, Task<TNext>> next)
            => chain is IChainFactoryAccessor accessor
                ? accessor.Factory.Create(next(chain.Result))
                : throw new Exception();

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChainAwaiter<T> chain, Func<T, Task<TNext>> next)
            => chain is IChainFactoryAccessor accessor
                ? accessor.Factory.Create(chain.Result.ContinueWith(async t => await next(await t)).ContinueWith(t => t.Result.Result))
                : throw new Exception();

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChainAwaiter<T> chain, Func<T, TNext> next)
            => chain is IChainFactoryAccessor accessor
                ? accessor.Factory.Create(chain.Result.ContinueWith(async t => next(await t)).ContinueWith(t => t.Result.Result))
                : throw new Exception();
    }
}
