using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    public interface IChainAsync<T>
    {
        IChainAsync<TNext> Then<TNext>(Func<T,TNext> next);

        IChainAsync<TNext> WaitThen<TNext>(Func<T,Task<TNext>> next);

        Task<T> Result { get; }
    }
}
