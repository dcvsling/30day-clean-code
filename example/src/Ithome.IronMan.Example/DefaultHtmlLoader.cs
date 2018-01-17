using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;

namespace Ithome.IronMan.Example
{
    public class DefaultHtmlLoader : IHtmlLoader
    {
        private HtmlDocument _html;
        public DefaultHtmlLoader(HtmlDocument html)
        {
            this._html = html;
        }
        public IEnumerable<HtmlNode> Load(Stream stream)
        {
            _html.Load(stream);
            return _html.DocumentNode.Descendants();
        }
    }
}