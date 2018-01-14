using System;

namespace Ithome.IronMan.Example.Plugins
{
    public class LazyChain<T> : IChain<T>, IChainFactoryAccessor
    {
        private readonly Func<T> _func;
        private readonly T _current;

        public LazyChain(ChainFactory factory,T current) : this(factory,() => current)
        {
            _current = current;
        }
        public LazyChain(ChainFactory factory,Func<T> func)
        {
            _func = func;
            Factory = factory;
        }

        public T Result => _func();

        public ChainFactory Factory { get; }
    }
}
