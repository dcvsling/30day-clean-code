using System;
using System.Linq;
namespace Ithome.IronMan.Example.Result
{
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
}