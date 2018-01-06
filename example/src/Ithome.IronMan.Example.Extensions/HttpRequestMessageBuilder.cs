
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;

namespace Ithome.IronMan.Example.Extensions
{
    public class HttpRequestMessageBuilder 
    {
        private IList<Action<HttpRequestMessage>> _configs;
        public HttpRequestMessageBuilder()
        {
            _configs = new List<Action<HttpRequestMessage>>();
        }

        public void Set(Action<HttpRequestMessage> config)
            => _configs.Add(config);
        
        public void SetUrl(Uri url) 
            => Set(req => req.RequestUri = (url ?? throw new ArgumentNullException(nameof(url))));

        public void SetMethod(HttpMethod method)
            => Set(req => req.Method = method ?? HttpMethod.Get);

        public void SetContent(HttpContent content)
            => Set(req => req.Content = content ?? new StringContent(string.Empty));

        public void ConfigureHeader(Action<HttpRequestHeaders> config)
            => Set(req => config?.Invoke(req.Headers));

        public HttpRequestMessage Build(Func<HttpRequestMessage> factory = default)
            => _configs.Aggregate(
                factory?.Invoke() ?? new HttpRequestMessage(),
                (req,config) => config(req));
    }
}
