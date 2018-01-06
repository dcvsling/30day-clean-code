
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Extensions
{
    public static class HttpRequestMessageBuilderExtensions 
    {
        public static HttpRequestMessageBuilder SetUrl(this HttpRequestMessageBuilder builder,string url)
        {
            builder.SetUrl(url.ToUrl());
            return builder;
        }
        public static HttpRequestMessageBuilder SetMethod(this HttpRequestMessageBuilder builder, string method)
        {
            builder.SetMethod(method?.ToHttpMethod());
            return builder;
        }

        public static HttpRequestMessageBuilder SetContent(this HttpRequestMessageBuilder builder,string content)
        {
            builder.SetContent(content.ToHttpContent());
            return builder;
        } 

        public static HttpRequestMessageBuilder SetContent(this HttpRequestMessageBuilder builder, Stream content)
        {
            builder.SetContent(content.ToHttpContent());
            return builder;
        }

        public static HttpRequestMessageBuilder SetHeaderBy(this HttpRequestMessageBuilder builder, Action<HttpRequestHeaders> config)
        {
            if(config == null) return builder;

            builder.ConfigureHeader(config);
            return builder;
        }

        public static HttpRequestMessageBuilder SetBy(this HttpRequestMessageBuilder builder,Action<HttpRequestMessageBuilder> config)
        {
            config?.Invoke(builder);
            return builder;
        }
    }
}
