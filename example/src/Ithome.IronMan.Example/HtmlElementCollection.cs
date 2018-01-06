using System.Collections;
using System.Collections.Generic;
namespace Ithome.IronMan.Example
{
    public class HtmlElementCollection : IHtmlElementCollection
    {
        public IEnumerable<HtmlElement> Elements { get; }

        public HtmlElementCollection(IEnumerable<HtmlElement> elements)
        {
            this.Elements = elements;
        }
        
        public IEnumerator<HtmlElement> GetEnumerator()
            => this.Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}