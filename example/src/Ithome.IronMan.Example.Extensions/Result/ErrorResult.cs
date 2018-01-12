using System.Linq;
using System;
namespace Ithome.IronMan.Example.Result
{
    public class ErrorResult : ErrorResult<IHtmlElementCollection>
    {
        public ErrorResult(Exception ex) : base(ex) {

        }
        public override IHtmlElementCollection Result
            => new HtmlElementCollection(Enumerable.Empty<HtmlElement>());
    }
}