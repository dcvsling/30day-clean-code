
using System.Linq;
using System.Reflection;
using System;

namespace Ithome.IronMan.Example.Plugins
{

    public class CallerContext
    {
        public CallerContext(object caller)
        {
            Target = caller;
        }
        public object Target { get; }
        public Type Type => Target.GetType();
        public Type From => Type.GenericTypeArguments.First();
        public Type To => Type.GenericTypeArguments.Last();
        public Delegate Delegate => (Delegate)Target;
        public MethodInfo CallerInfo => Delegate.Method;
        public string CallerName => CallerInfo.Name;
    }
}