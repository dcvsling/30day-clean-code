using System;
using System.Threading.Tasks;

namespace Fluent.Sync
{
    public interface IChain<T>
    {

        T Result { get; }
    }
}
