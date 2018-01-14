## Day-26 唯有掌握 Log，才能掌握 Exception
---
這是前篇最後提到的程式碼

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

依照前面所提到的擴充方法使用時機，這並不是一個足夠簡單的描述

甚至現在要加上```Try/Catch```這樣的語法，就已經足以證明了他並不簡單

更何況還要考量 Log 這樣的非主要功能的附加效果

所以先將這段程式變成合適的情境，再來思考如何解決這個問題吧

```csharp
public static Task<CrawlerResult> TryFindAsync(
    this ICrawler crawler,
    Action<HttpRequestMessageBuilder> config)
        => crawler.FindAsync(config);
```

## 那接下來的問題就是：如何在已經完成的程式碼中安插額外的功能？

可能很多人會想到[AOP](https://en.wikipedia.org/wiki/Aspect-oriented_programming)

這的確是一種解法，但是這項技術仍有不少開發者不願意使用

而且在我們還掌握整個 source，或是還沒開發完前

是否真的要這樣做，也許還有別的解決方案

所以就來談談 **如何將功能注入到你的程式吧**

以本篇的範例做範本

通常像是 ```Log``` ```Try/Catch``` 這樣的語法都是隨附在每一個運作流程中的

所以重新頗開```Crawler```來看一下常見的寫法 ~~以下為偷懶省略版~~

```csharp
    public Task<CrawlerResult> GetAsync(HttpRequestMessage request)
    {
        try
        {
            _logger.LogInformation($"Crawler : Input \r\n{request.ToString()} ");
            _logger.LogDegub("chain start with request");
            var req = Chain.StartBy(request);
            _logger.LogDegub("start request with httpclient");
            var res = req.Then(SendAsync);
            _logger.LogDegub("end request with httpclient");
            _logger.LogDegub("start with load content");
            var stream = res.Then(GetContentAsync);
            _logger.LogDegub("end with load content");
            _logger.LogDegub("start with load html from content");
            var htmls = stream.Then(LoadHtml);
            _logger.LogDegub("end with load html from content");
            _logger.LogDegub("start wrap html by CrawlerResult");
            var result = CrawlerResult.Ok(chain.Result);
            _logger.LogDegub("end wrap html by CrawlerResult");
            _logger.LogInformation($"Crawler : output \r\n{htmls.ToString} ");
            return result;
        }
        catch(Exception ex)
        {
            CrawlerResult.Error(ex);
            _logger.LogError(ex);
        }
    }
```

這豈不是把前面的努力都白費了嗎 ~~我承認我寫的很誇張　XD~~

所以這樣的寫法不僅徹底擾亂程式碼的可讀性

而且並無法去驗證每一個階段的行為與資料是否可行

至於改良它的方式，其實成品我們常常在用，但只是不清楚他是如何做到的

也就是 MVC 的 Filter 功能

先來簡單描述一下 Filter 的運作原理 [Interceptor](http://www.dofactory.com/net/interpreter-design-pattern)

簡單的範例如下：

```csharp
public class MyEcho
{
    public virtual string Echo(string input) => input;
}

public class MyEchoInterceptor : MyEcho
{
    public override string Echo(string input)
        => InterceptEcho(input);

    private string InterceptEcho(string input)
        => $"interceptor echo : {input}";
}
MyEcho echo = new MyEcho();
echo.Echo("test");
// test;
echo = new MyEchoInterceptor();
echo.Echo("test");
// interceptor echo : test;
```

這樣的範例也很常見，但就像是前面所提到的

解決方案往往都是需要經過客製化才會變成適用於自己的解決方案

所以接下來就來把這看似毫無用途的範例變成真正能解決我們困境的解決方案吧

## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 程式碼中應該減少靜態變數的直接使用，如必須使用的情境可以用 ```const```或```static readonly```來額外宣告
 - 當你程式在運行的過程中，每一次的靜態值都會被獨立建立，如果該值宣告的方法又被重複呼叫，那就很可觀了
 - Log, Exception 等等的Message可以透過Resource.resx這樣的資源檔或是其他類型的file 來統一定義
 - 狀態機(state)應該與其方法分開，盡可能減少方法內部存在自訂狀態的情境