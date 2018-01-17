using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ithome.IronMan.Example.Plugins
{
    public class Handler : IHandler
    {
        private readonly IServiceProvider _provider;

        public Handler(IServiceProvider provider)
        {
            _provider = provider;
        }
        public void Handle<T>(T context)
            => _provider.GetServices<IHandler<T>>()
                .Each(handler => handler.Handle(context));
    }
}