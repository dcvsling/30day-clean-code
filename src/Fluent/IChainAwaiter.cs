using System;
using System.Threading.Tasks;

namespace Fluent.Async
{
    public interface IChainAwaiter<T>
    {

        Task<T> Result { get; }
    }
}
