
using System.Linq;
using System.Reflection;
using System;

namespace Ithome.IronMan.Example.Plugins
{

    public class ResultContext
    {
        private readonly Func<object> _getter;

        public ResultContext(Func<object> getter)
        {
            _getter = getter;
        }

        public object Result => _getter();
        public Type Type => Result.GetType();
    }
}