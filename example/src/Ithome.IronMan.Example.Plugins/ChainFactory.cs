using System;
using System.Threading.Tasks;
namespace Ithome.IronMan.Example.Plugins
{
    public class ChainFactory : IChainFactory
    {
        private readonly IHandler _handler;

        public ChainFactory(IHandler handler)
        {
            _handler = handler;
        }

        public IChain<T> Create<T>(Action<FuncParser<T>> config)
        {
            Func<T> factory = null;
            config(new FuncParser<T>(_handler,func => factory = func));
            return new LazyChain<T>(this, Handle(factory));
        }

        public IChainAwaiter<T> Create<T>(Action<TaskParser<T>> config)
        {
            Task<T> task = null;
            config(new TaskParser<T>(_handler,t => task = t));
            return new ChainAwaiter<T>(this, Handle(task));
        }

        private Func<T> Handle<T>(Func<T> factory)   
        {
            var result = factory();
            _handler.Handle(new ResultContext(() => result));
            return () => result;
        }

        private Task<T> Handle<T>(Task<T> task)
        {
            _handler.Handle(new ResultContext(() => task.Result));
            return task;
        }
    }
}
