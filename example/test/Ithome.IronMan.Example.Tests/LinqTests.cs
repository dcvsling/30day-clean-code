using System;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
namespace Ithome.IronMan.Example.Tests
{
    public class LinqTests
    {
        [Fact]
        public Task LargeData_UseList_shouldThrowOutOfMemory()
        {
            var data = Enumerable.Range(1,10000000);
            Action actualRunner = () => data.ToList().ForEach(x => {});

            Assert.ThrowsAsync<OutOfMemoryException>(
                () => Task.Run(actualRunner));

            return Task.CompletedTask;
        }

        [Fact]
        public void LargeData_UseEnumerable_shouldSafe()
        {
            var data = Enumerable.Range(1,10000000);

            var actual = data.Aggregate((left,right) => right);

            Assert.Equal(10000000,actual);
        }
    }
}