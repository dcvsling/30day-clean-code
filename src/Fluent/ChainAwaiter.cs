using Fluent.Async;
using Fluent.Unit;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    public abstract class ChainAwaiter
    {
        public static IChainAwaiter<T> CreateAwaiter<T>(Task<T> task)
            => new ChainAwaiter<T>(task);
    }
}
