using System.Threading.Tasks;
namespace Ithome.IronMan.Example.Plugins
{
    public class Chain
    {
        private readonly IChainFactory _factory;

        public Chain(IChainFactory factory)
        {
            _factory = factory;
        }

        public IChain<T> StartBy<T>(T current)
            => new LazyChain<T>(_factory,current);

        public IChainAwaiter<T> StartBy<T>(Task<T> task)
            => new ChainAwaiter<T>(_factory,task);
    }
}
