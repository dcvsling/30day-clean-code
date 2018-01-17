using System;
using System.Threading.Tasks;
namespace Ithome.IronMan.Example.Plugins
{
    public class TaskParser<T>
    {
        private readonly IHandler _handler;
        private readonly Action<Task<T>> _callback;

        internal TaskParser(IHandler handler,Action<Task<T>> callback)
        {
            _handler = handler;
            _callback = callback;
        }

        public void Parse<TLast>(TLast arg, Func<TLast, Task<T>> func)
        {
            _handler.Handle(new CallerContext(func));
            _callback(func(arg));
        }

        public void Parse<TLast>(Task<TLast> arg, Func<TLast, T> func)
        {
            _handler.Handle(new CallerContext(func));
            _callback(Task.Run(async () => func(await arg)));
        }

        public void Parse<TLast>(Task<TLast> arg, Func<TLast, Task<T>> func)
        {
            _handler.Handle(new CallerContext(func));
            _callback(Task.Run(async () => await func(await arg)));
        }
    }
}
