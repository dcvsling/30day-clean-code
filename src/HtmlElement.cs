using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Ithome.IronMan.Example
{
    public class HtmlElement
    {
        private readonly HtmlNode _node;
        internal HtmlElement(HtmlNode node)
        {
            _node = node;
        }
        public string Name => _node.Name;
        public IDictionary<string,string> Attributes
            => _node.Attributes.ToDictionary(x => x.Name,x => x.Value);
        public IEnumerable<HtmlElement> Childrens
            => _node.ChildNodes.Select(x => new HtmlElement(x));
    }
}