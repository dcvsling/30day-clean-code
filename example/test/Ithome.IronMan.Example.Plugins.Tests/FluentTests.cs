using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Ithome.IronMan.Example.Plugins.Tests
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

        [Fact]
        async public Task ChainInvokeThen_1221_WillGet1221()
        {
            TextWriter writer = new StringWriter();
            var expect = "1221";

            var result = await Chain.StartBy(writer)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Then(WriteTwoAsync)
                .Then(WriteOne)
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

    public class FluentPipeTests
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

        private StringWriter WriteOne(TextWriter writer)
        {
            writer.Write("1");
            return new StringWriter(new StringBuilder(writer.ToString()));
        }
        async private Task<TextWriter> WriteTwoAsync(StringWriter writer)
        {
            await writer.WriteAsync("2");
            return writer;
        }
    }
}
