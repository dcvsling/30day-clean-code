## 可以自己取名?
---

前一篇最後提到了，一個有趣的方法名稱叫做 EnsureSuccessStatusCode

這個方法用名稱就讓我們確定裡面所做的事情是什麼

這也是其中一種主題相關的一種作法

所以我們可以回頭看我們新的爬蟲

透過一些描述強化，我們加入一些新的內部方法

```csharp
    public async Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
    {
        // 建立並設定 Request
        var req = CreateAndConfigure(CreateRequest,config);
        // 用http 送出Request並等待回應
        var res = await SendAsync(req);
        // 讀取Content
        var stream = await GetContent(res);
        // 讀取Html
        return LoadHtml(stream);
    }

    /// <summary>
    /// 建立空的 Request
    /// </summary>
    /// <returns></returns>
    private HttpRequestMessage CreateRequest()
        => new HttpRequestMessage();


    /// <summary>
    /// 建立並設定 Request
    /// </summary>
    /// <param name="factory">request factory</param>
    /// <param name="config">request config</param>
    /// <returns>requets</returns>
    private HttpRequestMessage CreateAndConfigure(Func<HttpRequestMessage> factory,Action<HttpRequestMessage> config)
    {
        var req = factory();
        config(req);
        return req;
    }

    /// <summary>
    /// 送出request
    /// </summary>
    /// <param name="request">request</param>
    /// <returns>response</returns>
    private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        => _http.SendAsync(request);

    /// <summary>
    /// 從確認成功的 response 中取得 content stream
    /// </summary>
    /// <param name="response">response</param>
    /// <returns>stream</returns>
    private Task<Stream> GetContent(HttpResponseMessage response)
        => response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

    /// <summary>
    /// 從 stream 中取得 html
    /// </summary>
    /// <param name="stream">stream</param>
    /// <returns>html structures</returns>
    private IEnumerable<HtmlElement> LoadHtml(Stream stream)
        => _html.Load(stream).Select(node => new HtmlElement(node));
    
```

透過這樣將各個功能重新組合成一個新的描述

可以使 GetAsync 這個方法內文變得更為清楚，雖然感覺還有點不整齊

分出來的方法皆為 private 的理由如下

 - 這些本來就是內部使用的，並不會因為他被分出來而就應該要被外部看到
 - 被外部看到就表示可以被外部使用，那就表示每公開一個就得多測試一個
 - 外部引用時並不需要知道它裡面詳細做了什麼事情
 - 外部也不需要煩惱到底該引用哪個，公開太多反而會擾亂引用者的選擇

而當每一個步驟都被這樣重組之後

再複雜的邏輯也可以清晰可見

不過我認為這樣還不夠

我不喜歡這樣每一行都是斷開的

Fluent Api 是讓整個程式都可以變成流暢的文句的關鍵之一

所以接下來我會再把它進化一次

備註
---

　- 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day5)
　- 關於委派的用法後續會再提到．