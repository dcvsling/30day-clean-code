using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    public interface IChain<T>
    {
        IChain<TNext> Then<TNext>(Func<T,TNext> next);

        IChainAsync<TNext> WaitThen<TNext>(Func<T,Task<TNext>> next);

        T Result { get; }
    }
}
