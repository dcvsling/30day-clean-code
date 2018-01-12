namespace Ithome.IronMan.Example.Result
{
    public class OkResult : OkResult<IHtmlElementCollection>
    {
        public OkResult(IHtmlElementCollection htmls) : base(htmls) 
        {
        }
    }
}
