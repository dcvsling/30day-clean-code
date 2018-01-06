## Day-17 左手是輔助還是擴充? 
---

在前面我們增加了幾個擴充的入口

我們來因應幾個常見的使用法來在一次的將入口擴充吧

## `請注意，此篇所介紹的內容並不需要總是完全實作`
## `應該要依照需求(含團隊隊員的需求)適量實作即可`

既然是擴充，就表示什麼時間什麼狀況都應該要可以加入

所以一開始就先將她從原本的專案上分開吧

直接額外再建立一個專案來參考原本的專案

而這個專案只是為了擴充爬蟲的方法

所以在專案後面像是測試一樣，加一個擴充的字眼(Extensions)

這樣就可以知道這個專案是專門為了前述的 Project 而存在的

再來繼前篇最後我們所加入的方法中，先在做一次調整

也就是把對於 HttpReqeustMessage 的設定在以擴充的方式把作法往外移

```csharp

    public static class CrawlerExtensions
    {
        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string url)
            => crawler.GetAsync(req => req.Set(new Uri(url))).ToCollectionsAsync();
    }

    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage Set(this HttpRequestMessage request,Uri url)
        {
            request.RequestUri = url;
            return request;
        }
    }

```

有時候我們不會只會抓method 為 Get的頁面

而且往往這種時候我們還需要帶一些內容進去

甚至還需要帶入Header 所以讓我們繼續把這幾種方法依序帶入

```csharp

    public static class CrawlerExtensions
    {
        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string url)
            => crawler.GetAsync(req => req.Set(new Uri(url))).ToCollectionsAsync();

        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string url,string method)
            => crawler.GetAsync(
                req => req.Set(new Uri(url))
                    .Set(new HttpMethod(method));

        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string method,string url,string content)
            => crawler.GetAsync(
                req => req.Set(new Uri(url))
                    .Set(new HttpMethod(method))
                    .Set(new StringContent(content)));

        public static Task<IHtmlElementCollection> GetAsync(this ICrawler crawler,string method,string url,Stream content)
            => crawler.GetAsync(
                req => req.Set(new Uri(url))
                    .Set(new HttpMethod(method))
                    .Set(new StreamContent(content)));

        public static Task<IHtmlElementCollection> GetAsync(
            this ICrawler crawler,
            string method,
            string url,
            string content,
            Action<HttpRequestHeaders> config)
            => crawler.GetAsync(
                req => req.Set(new Uri(url))
                    .Set(new HttpMethod(method))
                    .Set(new StringContent(content))
                    .SetBy(config));

        public static Task<IEnumerable<HtmlElement>> GetAsync(this ICrawler crawler,Action<HttpRequestMessage> config)
            => crawler.GetAsync(new HttpRequestMessage().SetBy(config));
        
        public static T SetBy<T>(this T target,Action<T> config)
        {
            config(target);
            return target;
        }
    }

    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage Set(this HttpRequestMessage request,Uri url)
        {
            request.RequestUri = url;
            return request;
        }

        public static HttpRequestMessage Set(this HttpRequestMessage request,HttpMethod method)
        {
            request.Method = method;
            return request;
        }

        public static HttpRequestMessage Set(this HttpRequestMessage request,HttpContent content)
        {
            request.Content = content;
            return request;
        }

        public static HttpRequestMessage SetBy(this HttpRequestMessage request,Action<HttpRequestHeaders> config)
        {
            if(config == null) return request;

            config(request.Headers);
            return request;
        }
    }

```

這樣就滿足前面說的四種狀況

所以現在我們甚至可以像是下面這樣去使用

```csharp
public static readonly string GOOGLE = "https://www.google.com";

crawler.GetAsync(GOOGLE);

crawler.GetAsync(GOOGLE,"Get");

crawler.GetAsync(GOOGLE,"Post","some hash......")

crawler.GetAsync(GOOGLE,"Post","some hash......",header => header[HttpRequestHeader.ContentLanguage] = "zh-tw")
```

看起來初步好像沒什麼問題

但其實回頭一看發現，雖然這樣做完了

但又變得開始看不懂了

即使我們將如何建立Request另外寫，似乎仍然不夠

而且，實際上設定也不夠全面

也沒有符合耳熟能詳的原則與規範 (ex: 太多 new 了)

這樣的東西下次再用可能又不足夠了吧

接下來得再將其好好調整一番吧

## 備註：
---

  - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
  - 因為本篇範例為未完成範例，故後續介紹完成後會提供完整範例 
  - 您可以嘗試把這些程式碼加入到範例來調整看看，也許你可能會做出更好的效果
  - 我們並不會一次就把程式碼寫到完美無缺，但也不需要等全部寫完才來整理