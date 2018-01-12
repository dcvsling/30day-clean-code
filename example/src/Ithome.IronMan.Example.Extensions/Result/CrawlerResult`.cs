using System;
using Ithome.IronMan.Example.Extensions;
namespace Ithome.IronMan.Example.Result
{
    public abstract class CrawlerResult<T>
    {
        public abstract CrawlerResult<T> OnError<TException>(Func<TException,Exception> handler)
            where TException : Exception;
        public abstract CrawlerResult<TNext> OnSuccess<TNext>(Func<T,TNext> map);
        public abstract T Result { get; }
    }

}