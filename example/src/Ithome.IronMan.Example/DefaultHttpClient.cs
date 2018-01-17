using System.Net.Http;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    public class DefaultHttpClient : IHttpClient
    {
        private readonly HttpClient _http;

        public DefaultHttpClient(HttpClient http)
        {
            this._http = http;
        }
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
            => _http.SendAsync(request);
    }
}