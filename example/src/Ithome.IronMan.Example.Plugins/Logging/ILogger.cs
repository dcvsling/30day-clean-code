using System;
using System.Collections.Generic;
using System.Text;

namespace Ithome.IronMan.Example.Plugins.Logging
{
    public interface ILogger<T>
    {
        void Write(Func<T,string> formatter);
    }
}
