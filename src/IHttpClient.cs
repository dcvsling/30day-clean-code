using System.Net.Http;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    /// <summary>
    /// Http用戶端
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// 以非同步方式發送Request 並等待Response
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}