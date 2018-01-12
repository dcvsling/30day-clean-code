using System;
using Ithome.IronMan.Example.Extensions;
namespace Ithome.IronMan.Example.Result
{
   
    public abstract class CrawlerResult : CrawlerResult<IHtmlElementCollection>
    {
        public static CrawlerResult<IHtmlElementCollection> Ok(IHtmlElementCollection htmls)
            => new OkResult(htmls);

        public static CrawlerResult<IHtmlElementCollection> Error(Exception exception)
            => new ErrorResult(exception);
    }
}