using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Threading.Tasks;
using System.IO;

namespace Ithome.IronMan.Example.Plugins.Tests
{
    public class HandlerTests
    {
        [Fact]
        async public Task LogWriter_UseAddCallerHandler_WriteCallerName()
        {
            var logwriter = new StringWriter();
            TextWriter writer = new StringWriter();
            var chain = new ServiceCollection()
                .AddChain()
                .AddCallerHandler(ctx => logwriter.WriteAsync($" {ctx.CallerName}"))
                .Services
                .BuildServiceProvider()
                .GetService<Chain>();
            var expect = $" {nameof(WriteOne)} {nameof(WriteTwoAsync)}";
            expect += expect;

            var result = await chain.StartBy(writer)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Result;

            var actual = logwriter.ToString();
            

            Assert.Equal(expect, actual);
        }

        [Fact]
        async public Task LogWriter_UseAddResultHandler_WriteResult()
        {
            var logwriter = new StringWriter();
            TextWriter writer = new StringWriter();
            var chain = new ServiceCollection()
                .AddChain()
                .AddResultHandler(ctx => logwriter.WriteAsync($" result is {ctx.Result} "))
                .Services
                .BuildServiceProvider()
                .GetService<Chain>();
            var expect = " result is 1  result is 12  result is 121  result is 1212 ";

            var result = await chain.StartBy(writer)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Result;

            var actual = logwriter.ToString();


            Assert.Equal(expect, actual);
        }

        [Fact]
        async public Task IntergrateTest_ChainHandler_ForLogging()
        {
            var logwriter = new StringWriter();
            TextWriter writer = new StringWriter();
            var chain = new ServiceCollection()
                .AddChain()
                .AddCallerHandler(ctx => logwriter.WriteAsync($"use {ctx.From.Name} invoke {ctx.CallerName} will get {ctx.To.Name} "))
                .AddResultHandler(ctx => logwriter.WriteLineAsync($"is {ctx.Result}"))
                .Services
                .BuildServiceProvider()
                .GetService<Chain>();
            var expect = "use TextWriter invoke WriteOne will get TextWriter is 1\r\nuse TextWriter invoke WriteTwoAsync will get Task`1 is 12\r\nuse TextWriter invoke WriteOne will get TextWriter is 121\r\nuse TextWriter invoke WriteTwoAsync will get Task`1 is 1212\r\n";

            var result = await chain.StartBy(writer)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Then(WriteOne)
                .Then(WriteTwoAsync)
                .Result;

            var actual = logwriter.ToString();

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
