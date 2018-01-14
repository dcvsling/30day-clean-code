using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{
    public interface IChain<T>
    {

        T Result { get; }
    }

    internal interface IChainFactoryAccessor
    {
        ChainFactory Factory { get; }
    }
}
