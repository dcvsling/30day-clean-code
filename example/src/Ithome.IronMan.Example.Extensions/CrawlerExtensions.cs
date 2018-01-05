
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Extensions
{
    public static class CrawlerExtensions 
    {
        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string url)
            => crawler.GetAsync(builder => builder.SetUrl(url));

        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string method,string url)
            => crawler.GetAsync(
               builder => builder.SetUrl(url)
                    .SetMethod(method));

        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,Action<HttpRequestMessageBuilder> config)
            => crawler.GetAsync(new HttpRequestMessageBuilder()
                    .SetBy(config)
                    .Build(CreateHttpRequestMessage))
                .ToCollectionsAsync();

        private static HttpRequestMessage CreateHttpRequestMessage() => new HttpRequestMessage();
    }
}
