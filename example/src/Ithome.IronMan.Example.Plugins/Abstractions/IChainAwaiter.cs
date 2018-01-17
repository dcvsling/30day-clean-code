
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{
    public interface IChainAwaiter<T> : IChainFactoryAccessor
    {

        Task<T> Result { get; }
    }
}
