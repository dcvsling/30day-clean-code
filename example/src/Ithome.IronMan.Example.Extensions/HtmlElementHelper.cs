using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Extensions
{
    internal static class HtmlElementHelper
    {
        internal static IHtmlElementCollection ToCollections(this IEnumerable<HtmlElement> elements)
            => new HtmlElementCollection(elements);

        async internal static Task<IHtmlElementCollection> ToCollectionsAsync(this Task<IEnumerable<HtmlElement>> task)
            => (await task).ToCollections();
    }
}
