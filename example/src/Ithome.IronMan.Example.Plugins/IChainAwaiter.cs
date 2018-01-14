
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{
    public interface IChainAwaiter<T>
    {

        Task<T> Result { get; }
    }
}
