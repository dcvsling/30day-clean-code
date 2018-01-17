using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ithome.IronMan.Example.Plugins.Tests
{
    public class FluentTests
    {
        [Fact]
        async public Task ChainInvokeThen_OneTwoTwice_WillGet1212()
        {
            var chain = new ServiceCollection()
                .AddChain()
                .Services
                .BuildServiceProvider()
                .GetService<Chain>();
            TextWriter writer = new StringWriter();
            var expect = "1212";

            var result = await chain.StartBy(writer)
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
