## Day-19 不知不覺就鍊成了!!
---

接下來繼續前篇未完成的部分

首先是Url，在大多數的狀況下都會是用String來設定

所以就額外加入一個擴充方法叫SetUrl

```csharp
    public static HttpRequestMessageBuilder SetUrl(this HttpRequestMessageBuilder builder,string url)
    {
        builder.SetUrl(url.ToUrl());
        return builder;
    }

    internal static Uri ToUrl(this string url)
         => new Uri(url);
```

再來是HttpMethod，同樣的較多狀況會使用字串來定義

所以同樣也用一個擴充方法來完成

```csharp
    public static HttpRequestMessageBuilder SetMethod(this HttpRequestMessageBuilder builder, string method)
    {
        builder.SetMethod(method.ToHttpMethod());
        return builder;
    }

    internal static HttpMethod ToHttpMethod(this string method)
        => new HttpMethod(method);
```

接下來Content 比較常見的兩個實作 String 和 Stream 也是各自建立自己的方法

```csharp
    public static HttpRequestMessageBuilder SetContent(this HttpRequestMessageBuilder builder,string content)
    {
        builder.SetContent(content.ToHttpContent());
        return builder;
    }

    internal static HttpContent ToHttpContent(this string str)
        => new StringContent(str);

    public static HttpRequestMessageBuilder SetContent(this HttpRequestMessageBuilder builder, Stream content)
    {
        builder.SetContent(content.ToHttpContent());
        return builder;
    }

    internal static HttpContent ToHttpContent(this Stream stream)
        => new StreamContent(stream);
```

Header的內容過多，他甚至可以額外在新建立一組Builder

所以就用前面常用的設定法來完成

```csharp
    public static HttpRequestMessageBuilder SetHeaderBy(this HttpRequestMessageBuilder builder, Action<HttpRequestHeaders> config)
    {
        if(config == null) return builder;

        builder.ConfigureHeader(config);
        return builder;
    }
```

而最後再用一個輔助的方法來達成讓Builder可以被連續設定

```csharp
    public static HttpRequestMessageBuilder SetBy(this HttpRequestMessageBuilder builder,Action<HttpRequestMessageBuilder> config)
    {
        config?.Invoke(builder);
        return builder;
    }
```

因此，Cralwer引用這樣的方法的方式就會是

```csharp
    public static Task<IHtmlElementCollection> GetByAsync(this ICrawler crawler,Action<HttpRequestMessageBuilder> config)
        => crawler.GetAsync(new HttpRequestMessageBuilder()
                .SetBy(config)
                .Build(CreateHttpRequestMessage))
            .ToCollectionsAsync();
```

所以就可以達成這次主題的目的

```csharp
    crawler.GetAsync(ConfigureRequest);

    public void ConfigureRequest(HttpRequestMessageBuilder req)
        => req.SetUrl("https://localhost/api")
            .SetMethod("Post")
            .SetContent(JsonConvert.SerializeObject(entitiy))
            .SetHeaderBy(ConfigureRequestHeader);

    public void ConfigureRequestHeader(HttpRequestHeader header)
        =>header.CacheControl
            .SetBy(ConfigureHeaderCacheControl);

    public void ConfigureHeaderCacheControl(CacheControlHeaderValue cache)
        => cache.NoCache = true;
```

繞了那麼遠，看看是否有達成目標，方法很簡單

跟著程式碼念一次看看對不對就知道了

```csharp
    // 爬蟲用設定的Request已非同步尋找 html集合 (FindByAsync的 return value)
    crawler.FindByAsync(ConfigureRequest);

    public void ConfigureRequest(HttpRequestMessageBuilder req)
            // 設置url 為 https://localhost/api 
        => req.SetUrl("https://localhost/api")
            // 設置HttpMethod 為 Post
            .SetMethod("Post")
            // 設置內容為 json 序列化 entity 的內容
            .SetContent(JsonConvert.SerializeObject(entitiy))
            // 用 ConfigureRequestHeader來設定 Header
            .SetHeaderBy(ConfigureRequestHeader);

    public void ConfigureRequestHeader(HttpRequestHeader header)
            // 用ConfigureHeaderCacheControl來設定 CacheControl
        =>header.CacheControl.SetBy(ConfigureHeaderCacheControl);

    public void ConfigureHeaderCacheControl(CacheControlHeaderValue cache)
            // 設定cache 的 NoCache為true
        => cache.NoCache = true;
```

整理文字如下

```
    內容大綱：
    爬蟲用設定的Request已非同步尋找 html集合 (FindByAsync的 return value)
    
    內容細節：
    設置url 為 https://localhost/api 
    設置HttpMethod 為 Post
    設置內容為 json 序列化 entity 的內容
    用 ConfigureRequestHeader來設定 Header
    用ConfigureHeaderCacheControl來設定 CacheControl
    設定cache 的 NoCache為true
```

如此一來外部也可以接得很清楚

## 備註：
---

 - 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day19)
 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 使用internal的擴充方法的理由是為了避免在使用時造成過多無謂的雜訊 (ex: string 並不是每一個都能轉為url or HttpMethod)
 - 所以同理，想要避免自己開發時造成自己過多的干擾，可以用private
 - SetContent 兩個方法內容雖然一樣，但是實際上他們運作的型別是不同的，利用類似的方式，可以達成不少描述過於複雜的情境(ex:很長的泛型)