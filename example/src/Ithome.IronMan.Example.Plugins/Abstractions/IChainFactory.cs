using System.Security.Cryptography.X509Certificates;
using System;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example.Plugins
{
    public interface IChainFactory
    {
        IChain<T> Create<T>(Action<FuncParser<T>> config);
        IChainAwaiter<T> Create<T>(Action<TaskParser<T>> config);
    }
}