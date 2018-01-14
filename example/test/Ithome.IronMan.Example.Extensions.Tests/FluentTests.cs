using System.Threading.Tasks;
using Xunit;
using System.IO;
using Ithome.IronMan.Example.Plugins;

namespace Ithome.IronMan.Example.Extensions.Tests
{
    public class FluentTests
    {
        [Fact]
        async public Task ChainInvokeThen_OneTwoTwice_WillGet1212()
        {
            TextWriter writer = new StringWriter();
            var expect = "1212";

            var result = await Chain.StartBy(writer)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Result;
            var actual = result.ToString();

            Assert.Equal(expect, actual);
        }

        private TextWriter WriteOne(TextWriter writer)
        {
            writer.Write("1");
            return writer;
        }
        async private Task<TextWriter> WriteTwoAsync(TextWriter writer)
        {
            await writer.WriteAsync("2");
            return writer;
        }
    }
}
