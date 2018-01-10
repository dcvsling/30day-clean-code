## Day-23 是通用，還是專用
---

有了前篇所描述的解決方案後

用相同的模型來創造一個屬於這個專案專屬的回傳型別吧

```csharp
public abstract class CrawlerResult<T>
{
    public abstract CrawlerResult<T> OnError<TException>(Func<TException,Exception> handler)
        where TException : Exception;
    public abstract CrawlerResult<T> OnSuccess<TResult>(Func<T,TResult> map);
    public abstract T Result { get; }
}
```

看起來有點不一樣　但仍有幾分神似

接著再來完成 Left 和 Right

```csharp
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
```
```csharp
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
```

作為輔助使用，所以在加一個呼叫類別

```csharp
    public abstract class CrawlerResult
    {
        public static CrawlerResult<IHtmlElementCollection> Ok(IHtmlElementCollection htmls)
            => new OkResult<IHtmlElementCollection>(htmls);

        public static CrawlerResult<IHtmlElementCollection> Error(Exception exception)
            => new ErrorResult<IHtmlElementCollection>(exception);
    }
```

別忘了改變回傳的結果，我們不應該直接改變 interface 與 class 

所以我們改用後來追加的擴充方法來完成

`這只是過程，請不要這樣寫擴充方法，後續會慢慢將他轉為合適的作法`

```csharp
async public static Task<CrawlerResult<IHtmlElementCollection>> TryFindAsync(
    this ICrawler crawler,
    Action<HttpRequestMessageBuilder> config)
    {
        try
        {
            return CrawlerResult.Ok(await crawler.FindAsync(config));
        }
        catch (Exception ex) 
        {
            return CrawlerResult.Error(ex);
        }
    }
```

如此一來我們就可以做到下面這件事情

```csharp

var result = await crawler.TryFindAsync(req => req.Set(GOOGLE));
var urls = result.OnError(ex => {
    logger.LogError(ex);
    return ex;
}).OnSuccess(htmls => htmls.Where(html => html.Name == "a")
    .SelectMany(htmls => htmls,(htmls,html) => htmls.Attributes["href"]))
    .Result;

```

或像先前一樣的作法

```csharp

var result = await crawler.TryFindAsync(req => req.SetUrl(GOOGLE));
var urls = result.OnError<Exception>(WriteErrorLog)
    .OnSuccess(GetUrls)
    .Result;

Assert.Single(urls);
Assert.Equal(GOOGLE, urls.Single());

private Exception WriteErrorLog(Exception ex)
{
    logger.LogError(ex);
    return ex;
}

private IEnumerable<string> GetUrls(IHtmlElementCollection htmls)
    => htmls.Where(html => html.Name == "a")
    .SelectMany(
        htmls => htmls,
        (htmls,html) => htmls.Attributes["href"]);
```

下一篇來[Diff](https://de.wikipedia.org/wiki/Diff) 一下看看插了那些以及為何要改變


## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 即便是在現在這個總是得開箱即用的環境，也不太可能對於解決方案是完全不用調整或設定，更何況是抽象的東西
 - 有些時候每一個人想法中的解決方案雖然以為都不同，但其實大致上不會偏離多遠，所以嘗試將一種情境加以抽象後，來運用在別的事問題上，有時候也可以得到不錯的成效
 - 盡可能不要以修改的方式改變原本的程式碼，現今有許多做法可以幫助我們保留原始的城市或是替換原始的城市，而這也是一個類別做一件事情的原因之一
 - 不要去任意加上 `Try/Catch`．除非你當下可以 `Handle` ，但往往可以 `Handle` 的例外都可以在前面被檢測出來，所以該 `throw` 時就應該要 `throw`
   - 如果你想要寫 `Log` 應統一交由最終輸出前的 `Logger` 來完成
   - 如果你不想讓例外跑到使用者端，應交由最終輸出前在統一 `Handle` 並轉為正常的錯誤訊息
 - 更不要在 在擴充方法中 包 `Try/Catch` 因為 `Exception` 會記錄 `traceStack` 停留在擴充方法內，但擴充方法基於前面所介紹的，只是一個連接語意，所以如果是可能會發生例外的一件事情，建議還是獨立的類別 較為妥當
 - 擴充方法應盡可能已足夠簡單到無需驗證的純函數來實作，就如同語意上，她只是將一個事物帶到另一個事物上