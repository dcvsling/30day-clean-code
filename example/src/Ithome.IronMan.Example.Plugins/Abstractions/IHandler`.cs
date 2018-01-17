using System;
using System.Collections.Generic;
using System.Text;

namespace Ithome.IronMan.Example.Plugins
{
    public interface IHandler<T>
    {
        void Handle(T context);
    }
}
