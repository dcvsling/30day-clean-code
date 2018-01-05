## Day-16 翻譯機?
---

一般來說呢，功能與功能之間的串接，多半都會借助於先前提到的[擴充方法](https://github.com/dcvsling/30day-clean-code/blob/Day16/topic/Day8.md)

基於[SOLID](https://en.wikipedia.org/wiki/SOLID_(object-oriented_design))的[責任單一原則](https://en.wikipedia.org/wiki/Single_responsibility_principle)

我們一個類別應該只做一件事情

所以接收各種Input 與輸出各種Output 就不應該出現在該類別上

因為這與該類別是毫無關係的

所以我們可以透過像是[Chain](https://github.com/dcvsling/30day-clean-code/blob/Day16/src/Fluent/Chain.cs)這樣的類別來加以輔助，而在C#則多一個擴充方法可以用

而這種類型的功能在.Net上最大宗的應該就是Linq體系的功能，像是Linq，Ix，Rx.....etc

不過這些功能的語意往往都過於程序話，也就是可能連工程師都不見得懂

所以用適量的包裹來整理我們想要描述的行為也是非常重要的

這裡就以前面的範例再來接續寫吧

首先，爬蟲是否應該要負責設定 Request

即使需求是這樣我們也不一定要完全按照需求上的那樣去定死，只要最後結果一致就好

所以將Input 稍作調整為普通的 HttpRequestMessage ，而註解也可以開始慢慢移除

```csharp

public interface ICrawler
{
    Task<IEnumerable<HtmlElement>> GetAsync(HttpRequestMessage request);
}
```
並且因為不再是設定Reqeust 而是直接使用Reqeust 

所以將Chain的Create(config) 改為 StartBy(target)

```csharp
public abstract class Chain
{
    public static IChain<T> StartBy<T>(T current)
        => new Chain<T>(current);
}

public class Crawler : ICrawler
{
    public Task<IEnumerable<HtmlElement>> GetAsync(HttpRequestMessage request)
        => Chain.StartBy(request)
        .Then(SendAsync)
        .Then(GetContentAsync)
        .Then(LoadHtml)
        .Result;

    //其餘省略
}
```

如此一來Crawler裡面的內容就會更清楚了

Chain 就表示連續作法，有Start 有Result

接著我們把原本的設定放進CrawlerExtensions

```csharp
public static class CrawlerExtensions
{
    public static Task<IEnumerable<HtmlElement>> GetAsync(this ICrawler crawler,Action<HttpRequestMessage> config)
        => crawler.GetAsync(new HttpRequestMessage().Set(config));
}
```

再來把複雜的IEnumerable<HtmlElement> 重新定義名稱，並且也為此作額外擴充

```csharp

    public interface IHtmlElementCollection : IEnumerable<HtmlElement>
    {
    }

    public class HtmlElementCollection : IHtmlElementCollection
    {
        public IEnumerable<HtmlElement> Elements { get; }

        public HtmlElementCollection(IEnumerable<HtmlElement> elements)
        {
            this.Elements = elements;
        }
        
        public IEnumerator<HtmlElement> GetEnumerator()
            => this.Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }

    public static class CrawlerExtensions
    {
        internal static IHtmlElementCollection ToCollections(this IEnumerable<HtmlElement> elements)
            => new HtmlElementCollection(elements);

        async internal static Task<IHtmlElementCollection> ToCollectionsAsync(
            this Task<IEnumerable<HtmlElement>> task)
            => (await task).ToCollections();
    }

```

最後我們來盡可能地為可能的設定加上貼心的擴充

```csharp

    public static class CrawlerExtensions
    {
        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string url)
            => crawler.GetAsync(req => req = new Uri(url)).ToCollectionsAsync();
    }
```

如此一來　我們就可以

```csharp

public static readonly string GOOGLE = "https://www.google.com";

crawler.GetAsync(GOOGLE);

```

當然如果只是這樣那就不用寫那麼麻煩了

這些都只是一個前置工作，為了讓辛辛苦苦寫出來的東西變得更加泛用，而且語意也會更加通順

## 備註：
---

  - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
  - 因為本篇範例為未完成範例，故後續介紹完成後會提供完整範例