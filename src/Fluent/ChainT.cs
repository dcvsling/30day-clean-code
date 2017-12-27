using Fluent.Sync;
using System;
using System.Threading.Tasks;
using Fluent.Async;

namespace Fluent.Unit
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
        
        public T Result => _current;
    }
}
