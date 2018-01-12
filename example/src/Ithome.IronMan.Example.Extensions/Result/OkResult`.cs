using System;
namespace Ithome.IronMan.Example.Result
{
    public class OkResult<T> : CrawlerResult<T>
    {
        private readonly T _result;

        public OkResult(T result)
        {
            _result = result;
        }
        public override T Result => _result;

        public override CrawlerResult<T> OnError<TException>(Func<TException, Exception> handler)
            => this;

        public override CrawlerResult<TNext> OnSuccess<TNext>(Func<T, TNext> map)
            => new OkResult<TNext>(map(_result));
    }
}