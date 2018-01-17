using System.Collections;
using Ithome.IronMan.Example.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ChainBuilder AddChain(this IServiceCollection services)
        {
            services.AddTransient<Chain>()
                .AddTransient<IChainFactory, ChainFactory>()
                .AddSingleton<IHandler, Handler>();
            return new ChainBuilder(services);
        }

        public static ChainBuilder AddResultHandler(this ChainBuilder builder,Action<ResultContext> handler)
        {
            builder.Services.AddSingleton<IHandler<ResultContext>>(new Handler<ResultContext>(handler));
            return builder;
        }
        public static ChainBuilder AddCallerHandler(this ChainBuilder builder, Action<CallerContext> handler)
        {
            builder.Services.AddSingleton<IHandler<CallerContext>>(new Handler<CallerContext>(handler));
            return builder;
        }
    }
}
