using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{

    public interface IChainFactoryAccessor
    {
        IChainFactory Factory { get; }
    }
}
