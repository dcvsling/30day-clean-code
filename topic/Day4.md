## Day-4 面子很重要
---

前篇最後提到了一個介面如下
```csharp
public interface ICrawler
{
    /// <summary>
    /// 依照提供的Request定義 => Action<HttpRequestMessage>
    /// 以非同步方式取得 　　　=> GetAsync
    /// Html物件序列          => IEnumerable<HtmlElement>
    /// </summary>
    /// <param name="config">Action[HttpRequestMessage]</param>
    /// <returns>IEnumerable[HtmlElement]</returns>
    Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config);
}
```
這樣的介面除了明確且簡單帶出爬蟲所作的事情外，還可以帶來不少好處

介面的優勢很多，例如符合隔離原則，易於測試，適用於TDD，好處太多了....

與目前主題最息息相關的就是他在描述上的簡化

在系統規劃時，初步的輪廓其實並不需要描述那麼多的細節

有時有多個選擇可以讓我們自由去選

甚至是還不知道要怎麼做時

就可以以這樣的方式簡單扼要地去宣告這一部分要做什麼

以這次的爬蟲來說的話

其實內部還可以再分為兩塊

```csharp
    /// <summary>
    /// Http用戶端
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// 以非同步方式發送Request 並等待Response
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
    /// <summary>
    /// 讀取Stream中的Html
    /// </summary>
    public interface IHtmlLoader
    {
        /// <summary>
        /// 從Stream中讀取Html節點
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>html節點</returns>
        IEnumerable<HtmlNode> Load(Stream stream);
    }
```

而新的爬蟲則這樣寫
```csharp
    public class NewCrawler : ICrawler
    {
        private IHttpClient _http;
        private IHtmlLoader _html;
        public NewCrawler(IHttpClient http,IHtmlLoader html)
        {
            this._http = http;
            this._html = html;
        }

        public async Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
        {
            // 建立request
            var req = new HttpRequestMessage();
            // 設定request
            config(req);
            // 用http 送出Request並等待回應
            var res = await _http.SendAsync(req);
            // 確認回應為成功狀態後
            var stream = await res.EnsureSuccessStatusCode()
                // 讀取Conten的Stream並等待結果
                .Content.ReadAsStreamAsync();
            //回傳html讀取Stream後
            return _html.Load(stream)
                //用各html節點建立的Html結構的序列
                .Select(node => new HtmlElement(node));
        }
    }
```

感覺似乎還不夠，裡面還有很多事情都與爬蟲無關

不過同時也看到了解決方法，也就是EnsureSuccessStatusCode()

這個方法用一個方法名稱就講完一堆動作

像是取得response的status code, 

檢查是否為成功的code ,如果是就回傳自己, 

如果不是就Throw Exception

像是後面這一段就是細節，而細節不屬於我們現在這個內容中所需要詳述的事情

所以我們同樣可以用一樣的方式將一些細節再次移出

備註
---

　- 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day4)
　- 關於介面的應用過於廣泛，此處僅介紹介面與主題相關之內容．