using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace Ithome.IronMan.Example
{
    public class Crawler
    {
        public List<HtmlNode> Start(string url)
        {
            var res = new HttpClient().GetAsync(url).Result;
            var stream = res.Content.ReadAsStreamAsync().Result;
            var docs = new HtmlDocument();
            docs.Load(stream);
            return docs.DocumentNode.Elements("a").ToList();
        }
    }
}
