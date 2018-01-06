using System;
using Fluent.Async;
using Fluent.Unit;

namespace Fluent.Sync
{
    public static class ChainHelper
    {
        /// <summary>
        /// 接著走下一步到下一個階段
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>下一個階段</returns>
        public static IChain<TNext> Then<T,TNext>(this IChain<T> chain,Func<T, TNext> next)
            => new Chain<TNext>(next(chain.Result));

        /// <summary>
        /// 用等待的結果，接著走下一步到下一個階段
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public static IChainAwaiter<TNext> Then<T,TNext>(this IChainAwaiter<T> chain,Func<T,TNext> next)
            => new ChainAwaiter<TNext>(chain.Result.ContinueWith(t => next(t.Result)));

        
    }
}
