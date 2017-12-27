using Fluent.Async;
using System;
using System.Threading.Tasks;

namespace Fluent.Unit
{
    /// <summary>
    /// 非同步的方法責任練
    /// </summary>
    public class ChainAwaiter<T> : IChainAwaiter<T>
    {
        private Task<T> _task;
        internal ChainAwaiter(Task<T> task)
        {
            _task = task;
        }
        
        public Task<T> Result => _task;
    }
}
