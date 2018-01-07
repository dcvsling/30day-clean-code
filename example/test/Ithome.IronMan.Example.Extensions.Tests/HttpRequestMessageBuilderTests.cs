using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Ithome.IronMan.Example.Extensions.Tests
{
    public class HttpRequestMessageBuilderTests
    {
        public const string LINK_CONTENT = @"<a href='https://www.google.com/' />";
        public const string DIV_CONTENT = @"<div></div>";
        [Fact]
        public void SetUrl_WithBuilder_ShouldGetGoogleRequest()
        {
            var expect = "https://www.google.com/";
            var builder = new HttpRequestMessageBuilder();

            var actual = builder.SetUrl(expect).Build().RequestUri.AbsoluteUri;

            Assert.Equal(expect,actual);
        }

        [Fact]
        public void SetMethod_WithBuilder_ShouldGetHttpGet()
        {
            var url = "https://www.google.com/";
            var expect = "GET";
            var builder = new HttpRequestMessageBuilder();

            var actual = builder.SetUrl(url).SetMethod(expect).Build().Method.Method;

            Assert.Equal(expect,actual);
        }

        [Fact]
        async public Task SetString_WithBuilder_ShouldGetOk()
        {
            var url = "https://www.google.com/";
            var method = "GET";
            var expect = "Ok";
            var builder = new HttpRequestMessageBuilder();

            var actual = await builder.SetUrl(url)
                .SetMethod(method)
                .SetContent(expect)
                .Build()
                .Content
                .ReadAsStringAsync();

            Assert.Equal(expect,actual);
        }

        [Fact]
        async public Task SetStream_WithBuilder_ShouldGetOk()
        {
            var url = "https://www.google.com/";
            var method = "GET";
            var expect = "Ok";
            var builder = new HttpRequestMessageBuilder();
            var bytes = Encoding.UTF8.GetBytes(expect);
            using(var stream = new MemoryStream(bytes))
            {
                var result = await builder.SetUrl(url)
                    .SetMethod(method)
                    .SetContent(stream)
                    .Build()
                    .Content
                    .ReadAsStreamAsync();

                Assert.Equal(bytes.Length,result.Length);
                var actual = await new StreamReader(result).ReadToEndAsync();

                Assert.Equal(expect,actual);
            }
        }

        [Fact]
        public void ConfigHeader_WithBuilder_ShouldBeHeaderOk()
        {
            var url = "https://www.google.com/";
            var method = "GET";
            var content = "test string content";
            var testkey = "testing";
            var expect = "header ok";
            var builder = new HttpRequestMessageBuilder();

            var actuals = builder.SetUrl(url)
                .SetMethod(method)
                .SetContent(content)
                .SetHeaderBy(header => header.Add(testkey,expect))
                .SetHeaderBy(header => header.Add(testkey,expect))
                .Build()
                .Headers.GetValues(testkey);

            Assert.Collection(
                actuals,
                actual => Assert.Equal(expect,actual),
                actual => Assert.Equal(expect,actual));
        }
    }
}
