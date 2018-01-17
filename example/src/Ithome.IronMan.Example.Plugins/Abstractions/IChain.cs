using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{
    public interface IChain<T> : IChainFactoryAccessor
    {

        T Result { get; }
    }
}
