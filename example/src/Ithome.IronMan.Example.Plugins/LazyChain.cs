using System;

namespace Ithome.IronMan.Example.Plugins
{
    public class LazyChain<T> : IChain<T>
    {
        private readonly Func<T> _func;
        private readonly T _current;

        public LazyChain(IChainFactory factory,T current) : this(factory,() => current)
        {
            _current = current;
        }
        public LazyChain(IChainFactory factory,Func<T> func)
        {
            _func = func;
            Factory = factory;
        }

        public T Result => _func();

        public IChainFactory Factory { get; }
    }
}
