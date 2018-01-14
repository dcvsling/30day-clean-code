using Ithome.IronMan.Example.Plugins.Handlers;
using Ithome.IronMan.Example.Plugins.Logging;
using System;
using System.Threading.Tasks;
namespace Ithome.IronMan.Example.Plugins
{
    public abstract class Chain
    {
        public static IChain<T> StartBy<T>(T current)
            => new LazyChain<T>(current);

        public static IChainAwaiter<T> StartBy<T>(Task<T> task)
            => new LazyChainAwaiter<T>(task);
    }

    public class ChainFactory
    {
        private readonly IHandlerFactory _handler;
        private readonly ILoggerFactory _logger;

        public ChainFactory(ILoggerFactory logger,IHandlerFactory handler)
        {
            _handler = handler;
            _logger = logger;
        }

        public IChain<T> Create<T>(Func<T> factory)
        {
            return new LazyChain<T>(factory);
        }

        public IChainAwaiter<T> Create<T>(Task<T> task)
        {
            return new ChainAwaiter<T>(task);
        }
    }

    public interface ILoggerFactory
    {
        ILogger<T> Create<T>();
    }

    public interface IHandlerFactory
    {
        IHandler<T> Create<T>();
    }
}
