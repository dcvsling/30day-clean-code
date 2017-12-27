using Fluent.Sync;
using Fluent.Unit;
using System;
using System.Threading.Tasks;
namespace Fluent.Async
{

    public static class ChainHelper
    {
        /// <summary>
        /// 接著走下一步到下一個非同步階段
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>分同步階段</returns>
        public static IChainAwaiter<TNext> Then<T,TNext>(this IChain<T> chain,Func<T, Task<TNext>> next)
            => new ChainAwaiter<TNext>(next(chain.Result));

        /// <summary>
        /// 用等待的結果，接著走下一步到下一個非同步階段
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public static IChainAwaiter<TNext> Then<T,TNext>(
            this IChainAwaiter<T> chain,
            Func<T, Task<TNext>> next)
            => new ChainAwaiter<TNext>(chain.Result.ContinueWith(async t => await next(await t)).ContinueWith(t => t.Result.Result));
    }
}
