using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{

    internal class ChainAwaiter<T> : IChainAwaiter<T>, IChainFactoryAccessor
    {
        public ChainAwaiter(IChainFactory factory,Task<T> task)
        {
            Result = task;
            Factory = factory;
        }
        public Task<T> Result { get; }
        public IChainFactory Factory { get; }
    }
}
