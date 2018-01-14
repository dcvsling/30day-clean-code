using System;
using System.Collections.Generic;
using System.Text;

namespace Ithome.IronMan.Example.Plugins.Handlers
{
    public interface IHandler<T>
    {
        void Handle(Action<T> handler);
    }
}
