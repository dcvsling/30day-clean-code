using System;
using System.Collections.Generic;
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
            var req = new HttpRequestMessage();
            config(req);
            var res = await _http.SendAsync(req);
            var stream = await res
                .EnsureSuccessStatusCode()
                .Content
                .ReadAsStreamAsync();
            return _html
                .Load(stream)
                .Select(node => new HtmlElement(node));
        }
    }
}
