using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    public class NewCrawler : ICrawler
    {
        private IHttpClient _http;
        private IHtmlLoader _html;
        public NewCrawler(IHttpClient http,IHtmlLoader html)
        {
            this._http = http;
            this._html = html;
        }

        public async Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
        {
            // 建立並設定 Request
            var req = CreateAndConfigure(CreateRequest,config);
            // 用http 送出Request並等待回應
            var res = await SendAsync(req);
            // 讀取Content
            var stream = await GetContent(res);
            // 讀取Html
            return LoadHtml(stream);
        }

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
        private HttpRequestMessage CreateAndConfigure(Func<HttpRequestMessage> factory,Action<HttpRequestMessage> config)
        {
            var req = factory();
            config(req);
            return req;
        }

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
        private Task<Stream> GetContent(HttpResponseMessage response)
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
