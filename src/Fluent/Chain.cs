using System.Threading.Tasks;
using Fluent.Async;
using Fluent.Sync;
using Fluent.Unit;
namespace Ithome.IronMan.Example
{
    public abstract class Chain
    {
        public static IChain<T> Create<T>(T current)
            => new Chain<T>(current);

        public static IChainAwaiter<T> Create<T>(Task<T> task)
            => new ChainAwaiter<T>(task);
    }
}
