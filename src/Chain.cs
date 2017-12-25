using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    /// <summary>
    /// 同步的方法責任練
    /// </summary>
    public class Chain<T> : IChain<T>
    {
        private T _current;
        internal Chain(T current)
        {
            _current = current;
        }
        /// <summary>
        /// 接著走下一步到下一個階段
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>下一個階段</returns>
        public IChain<TNext> Then<TNext>(Func<T, TNext> next)
            => new Chain<TNext>(GetNextValue<TNext>(next));

        /// <summary>
        /// 接著走下一步到下一個非同步階段
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>分同步階段</returns>
        public IChainAsync<TNext> ThenAsync<TNext>(Func<T, Task<TNext>> next)
            => new ChainAsync<TNext>(Task.Run(() => next(_current)));

        /// <summary>
        /// 取得下一個階段的回傳值
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>回傳值</returns>
        private TNext GetNextValue<TNext>(Func<T,TNext> next)
            => next(_current);

        /// <summary>
        /// 取得非同步的下一個階段的回傳值
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>非同步的回傳值</returns>
        private Task<TNext> GetNextValueAsync<TNext>(Func<T, Task<TNext>> next)
            => next(_current);
        public T Result => _current;
    }
}
