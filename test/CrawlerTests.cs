using System;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Threading;
using HtmlAgilityPack;
using System.Linq;

namespace Ithome.IronMan.Example.Tests
{
    public class CrawlerTests
    {
        public const string LINK_CONTENT = @"<a href='https://www.google.com' />";
        public const string DIV_CONTENT = @"<div></div>";
        [Fact]
        async public Task GetElement_FromOkHandler_HasHyperLink()
        {
            var crawler = CreateCrawler<OkHandler>();
            var request = ConfigureRequest(WithContent(LINK_CONTENT));

            var result = await crawler.GetAsync(request);

            Assert.Collection(result,HasHyperLink);
        }
        [Fact]
        async public Task GetElement_FromOkHandler_HasTwoHyperLink()
        {
            var crawler = CreateCrawler<OkHandler>();
            var request = ConfigureRequest(WithContent(LINK_CONTENT,LINK_CONTENT));

            var result = await crawler.GetAsync(request);

            Assert.Collection(result, HasHyperLink, HasHyperLink);
        }

        [Fact]
        async public Task GetElement_FromOkHandler_HasHyperLinkAndDiv()
        {
            var crawler = CreateCrawler<OkHandler>();
            var request = ConfigureRequest(WithContent(LINK_CONTENT,DIV_CONTENT));

            var result = await crawler.GetAsync(request);

            Assert.Collection(result, HasHyperLink, HasDiv);
        }

        [Fact]
        async public Task GetElement_FromErrorHandler_WillThrowHttpRequestException()
        {
            var crawler = CreateCrawler<ErrorHandler>();

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => crawler.GetAsync(ConfigureRequest(WithContent(LINK_CONTENT))));
        }

        private void HasHyperLink(HtmlElement actual)
        {
            Assert.Equal("a", actual.Name);
            Assert.Equal("href", actual.Attributes.Single().Key);
            Assert.Equal("https://www.google.com", actual.Attributes.Single().Value);
        }

        private void HasDiv(HtmlElement actual)
        {
            Assert.Equal("div", actual.Name);
        }

        private IHttpClient CreateHttpClient(HttpMessageHandler handler)
            => new TestHttpClient(handler);

        private IHtmlLoader CreateHtmlLoader()
            => new TestHtmlLoader();

        private ICrawler CreateCrawler<THandler>() where THandler : HttpMessageHandler,new()
            => new Crawler(
                CreateHttpClient(new THandler()),
                CreateHtmlLoader());

        private Action<HttpRequestMessage> WithContent(params string[] contents)
            => req =>
            {
                req.Content = new StringContent(String.Join(string.Empty,contents));
            };

        private Action<HttpRequestMessage> ConfigureRequest(Action<HttpRequestMessage> config)
            => req =>
            {
                req.RequestUri = new Uri("https://127.0.0.1/");
                req.Method = HttpMethod.Get;
                config(req);
            };
    }

    public class TestHttpClient : IHttpClient
    {
        private HttpMessageHandler _handler;
        
        public TestHttpClient(HttpMessageHandler handler)
        {
            _handler = handler;
        }


        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
            => new HttpClient(_handler).SendAsync(request);
    }

    public class TestHtmlLoader : IHtmlLoader
    {
        public IEnumerable<HtmlNode> Load(Stream stream)
        {
            var docs = new HtmlDocument();
            docs.Load(stream);
            return docs.DocumentNode.Descendants();
        }
    }
    public class OkHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {Content =  request.Content});
    }

    public class ErrorHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
    }
}
