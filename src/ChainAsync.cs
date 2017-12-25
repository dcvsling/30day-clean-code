using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    /// <summary>
    /// 非同步的方法責任練
    /// </summary>
    public class ChainAsync<T> : IChainAsync<T>
    {
        private Task<T> _task;
        internal ChainAsync(Task<T> task)
        {
            _task = task;
        }

        /// <summary>
        /// 用等待的結果，接著走下一步到下一個非同步階段
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public IChainAsync<TNext> WaitThen<TNext>(Func<T,Task<TNext>> next)
            => new ChainAsync<TNext>(GetNextValueAsync<TNext>(next));

        /// <summary>
        /// 用等待的結果，接著走下一步到下一個階段
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public IChainAsync<TNext> Then<TNext>(Func<T,TNext> next)
            => new ChainAsync<TNext>(GetNextValue<TNext>(next));

        /// <summary>
        /// 取得下一個結果
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        async private Task<TNext> GetNextValue<TNext>(Func<T,TNext> next)
            => next(await _task);

        /// <summary>
        /// 取得下一個非同步的結果
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        async private Task<TNext> GetNextValueAsync<TNext>(Func<T, Task<TNext>> next)
            => await next(await _task);
            
        public Task<T> Result => _task;
    }
}
