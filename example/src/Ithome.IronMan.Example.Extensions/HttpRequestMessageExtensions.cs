
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Ithome.IronMan.Example.Extensions
{
    internal static class HttpRequestMessageExtensions 
    {
        internal static HttpContent ToHttpContent(this string str)
            => new StringContent(str);
        internal static HttpContent ToHttpContent(this Stream stream)
            => new StreamContent(stream);
        internal static Uri ToUrl(this string url)
            => new Uri(url);
        internal static HttpMethod ToHttpMethod(this string method)
            => new HttpMethod(method);
    }
}
