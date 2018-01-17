
using System;
using System.Threading.Tasks;
namespace Ithome.IronMan.Example.Plugins
{
    public class FuncParser<T>
    {
        private readonly IHandler _handler;
        private readonly Action<Func<T>> _callback;

        internal FuncParser(IHandler handler,Action<Func<T>> callback)
        {
            _handler = handler;
            _callback = callback;
        }

        public void Parse<TLast>(TLast arg,Func<TLast, T> func)
            => _callback(Handle(func,arg));

        private Func<T> Handle<TLast>(Func<TLast, T> func,TLast arg)
        {
            _handler.Handle(new CallerContext(func));
            return () => func(arg);
        } 
    }
}
