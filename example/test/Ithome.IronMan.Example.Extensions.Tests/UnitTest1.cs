using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
namespace Ithome.IronMan.Example.Extensions.Tests
{
    public class HttpRequestMessageBuilderTests
    {
        public const string LINK_CONTENT = @"<a href='https://www.google.com' />";
        public const string DIV_CONTENT = @"<div></div>";
        [Fact]
        async public Task CreateReqeust_FromBuilder_()
        {
            var crawler = CreateCrawler<OkHandler>();
            var request = CreateRequest(WithContent(LINK_CONTENT));
            
            var result = await crawler.GetAsync(request);

            Assert.Collection(result,HasHyperLink);
        }
    }
}
