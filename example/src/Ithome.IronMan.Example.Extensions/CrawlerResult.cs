using System;
using System.Linq;

namespace Ithome.IronMan.Example.Extensions
{
    public abstract class CrawlerResult<T>
    {
        public abstract CrawlerResult<T> OnError<TException>(Func<TException,Exception> handler)
            where TException : Exception;
        public abstract CrawlerResult<TNext> OnSuccess<TNext>(Func<T,TNext> map);
        public abstract T Result { get; }
    }

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

    public class ErrorResult<T> : CrawlerResult<T>
    {
        private readonly Exception _exception;

        public ErrorResult(Exception exception)
        {
            this._exception = exception;
        }
        public override T Result => ThrowIfExistException(CreateEmpty);

        public override CrawlerResult<T> OnError<TException>(Func<TException, Exception> handler)
            => new ErrorResult<T>(_exception is TException ex ? handler(ex) : _exception);

        public override CrawlerResult<TNext> OnSuccess<TNext>(Func<T, TNext> map)
            => new ErrorResult<TNext>(_exception);

        private T CreateEmpty()
            => new HtmlElementCollection(Enumerable.Empty<HtmlElement>()) is T result ? result : default;

        
        private T ThrowIfExistException(Func<T> factory)
            => _exception == null ? DefaultIfNotHtmlCollection(factory) : throw _exception;

        private T DefaultIfNotHtmlCollection(Func<T> factory)
            => typeof(T) == typeof(IHtmlElementCollection)
                ? factory()
                : default;
    }

    public class CrawlerResult : CrawlerResult<IHtmlElementCollection>
    {
        public static CrawlerResult<IHtmlElementCollection> Ok(IHtmlElementCollection htmls)
            => new OkResult<IHtmlElementCollection>(htmls);

        public static CrawlerResult<IHtmlElementCollection> Error(Exception exception)
            => new ErrorResult<IHtmlElementCollection>(exception);
        private readonly IHtmlElementCollection _result;

        public CrawlerResult(IHtmlElementCollection result)
        {
            _result = result;
        }
        public override IHtmlElementCollection Result => _result;

        public override CrawlerResult<IHtmlElementCollection> OnError<TException>(Func<TException, Exception> handler)
            => this;

        public override CrawlerResult<TNext> OnSuccess<TNext>(Func<IHtmlElementCollection, TNext> map)
            => new OkResult<TNext>(map(_result));
    }
}