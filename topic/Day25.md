## Day-25 在這瞬間！我要翻開在程式中的技術債
---

基於前篇所提及的說法

首先先將 ```CrawlerResult<IHtmlElementCollection>``` 換掉

並且以先前做的 ```CrawlerResult```作為預設的開始結果

```csharp
    public class CrawlerResult : CrawlerResult<IHtmlElementCollection>
    {
        public static CrawlerResult Ok(IHtmlElementCollection htmls)
            => new OkResult(htmls);

        public static CrawlerResult Error(Exception exception)
            => new ErrorResult(exception);

        public abstract IHtmlElementCollection Result { get; }

        public abstract CrawlerResult<IHtmlElementCollection> OnError<TException>(Func<TException, Exception> handler)

        public abstract CrawlerResult<TNext> OnSuccess<TNext>(Func<IHtmlElementCollection, TNext> map)
    }

```

如果要福胡　Ok 和　Error 兩個方法都能回傳正確的型別

所以我們還需要特別建立各自的初始預設型別

```csharp
public class OkResult : OkResult<IHtmlElementCollection>
{
    public OkResult(IHtmlElementCollection htmls) : base(htmls) { }
}

public class ErrorResult : ErrorResult<IHtmlElementCollection>
{
    public ErrorResult(Exceoption ex) : base(ex) { }
}
```

再因為是初始物件的緣故

也許可能有些做法適合 throw 有些做法適合回傳空序列

以這次的案例來說，預設是只要仍收到例外物件就會 throw 的情況

而我們期望她回傳空序列

就可以在對 Result 這個屬性直接定義預設行為或預設值

```csharp
public class ErrorResult : ErrorResult<IHtmlElementCollection>
{
    public ErrorResult(Exceoption ex) : base(ex) { }

    public override IHtmlElementCollection Result
            => new HtmlElementCollection(Enumerable.Empty<HtmlElement>());
}
```

或是你可以反過來利用原本既有的 OkResult 來做一個新的 Result Type 叫做 Empty

```csharp
public class EmptyResult : OkResult<IHtmlElementCollection>
{
    public EmptyResult() : base(new HtmlElementCollection(Enumerable.Empty<HtmlElement>()))
    {
    }
}
```

而最後的一塊拼圖，也就是我說```別這樣寫的區塊```

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

利用這最後一塊拼圖來掀開一個現實面的問題吧

## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - Ｑ：泛型型別與同名非泛型型別之間有什麼關係
   - Ａ：沒有任何關係，他們是兩個完全不同的型別，就像是同名不同姓的兩個人，也通常是毫無關係
 - Ｑ：那　```Class<T> : Class``` 與 ```Class : Class<OtherClass>``` 的差別是？
   - Ａ：前者叫做衍生，後者叫做攀關係
     - 基於我們明確確定 ```OkResult``` is　```OkResult<IHtmlElementCollection>```，所以才會以繼承的方式去實現它，這種時候會比用強制轉型之類的還安全
