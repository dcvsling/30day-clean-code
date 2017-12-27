using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fluent.Sync;
namespace Ithome.IronMan.Example
{

    using Fluent.Async;
    public class NewCrawler : ICrawler
    {
        private IHttpClient _http;
        private IHtmlLoader _html;
        private Func<HttpRequestMessage> _request;
        public NewCrawler(IHttpClient http,IHtmlLoader html)
        {
            this._http = http;
            this._html = html;
            this._request = CreateRequest;
        }

        public Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
            => Chain.Create(_request())
            // IChain<HttpRequestMessage>.ThenAsync<Task<HttpResponseMessage>>
            .Then(SendAsync)
            // IChainAsync<HttpResponseMessage>.WaitThen<Task<Stream>>
            .Then(GetContentAsync)
            // IChainAsync<Stream>.Then<IEnumerable<HtmlElement>>
            .Then(LoadHtml)
            // Result = Task<IEnumerable<HtmlElement>>
            .Result;

        /// <summary> 
        /// 建立空的 Request
        /// </summary>
        /// <returns></returns>
        private HttpRequestMessage CreateRequest()
            => new HttpRequestMessage();

        /// <summary>
        /// 建立並設定 Request
        /// </summary>
        /// <param name="factory">request factory</param>
        /// <param name="config">request config</param>
        /// <returns>requets</returns>
        private Func<HttpRequestMessage,HttpRequestMessage> ConfigureBy(Action<HttpRequestMessage> config)
            => req => {
                config(req);
                return req;
            };

        /// <summary>
        /// 送出request
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
            => _http.SendAsync(request);

        /// <summary>
        /// 從response 中取得 content stream
        /// </summary>
        /// <param name="response">response</param>
        /// <returns>stream</returns>
        private Task<Stream> GetContentAsync(HttpResponseMessage response)
            => response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        /// <summary>
        /// 從 stream 中取得 html
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>html structures</returns>
        private IEnumerable<HtmlElement> LoadHtml(Stream stream)
            => _html.Load(stream).Select(node => new HtmlElement(node));
    }
}
