using System;

namespace Ithome.IronMan.Example.Plugins
{
    public class Handler<T> : IHandler<T>
    {
        private readonly Action<T> _handler;

        public Handler() : this(_ => { })
        {

        }

        public Handler(Action<T> handler)
        {
            _handler = handler;
        }
        public void Handle(T context)
            => _handler(context);
    }
}