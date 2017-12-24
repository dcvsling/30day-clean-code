# 一行解決? 連續技當然算一招
---

接續前一篇說，我還希望他在更整齊精準一些，前篇最後爬蟲如下
```csharp
    public async Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
    {
        // 建立並設定 Request
        var req = CreateAndConfigure(CreateRequest,config);
        // 用http 送出Request並等待回應
        var res = await SendAsync(req);
        // 讀取Content [前篇筆誤，非同步方法要寫Async字尾]
        var stream = await GetContentAsync(res);
        // 讀取Html
        return LoadHtml(stream);
    }
```

接下來借用一點TDD的概念，先寫出我想要的樣，在掛上之前的註解內容ㄓ

```csharp
    /// <summary>
    /// 依照提供的Request定義以非同步方式取得Html物件序列
    /// </summary>
    /// <param name="config">Action[HttpRequestMessage]</param>
    /// <returns>IEnumerable[HtmlElement]</returns>
    public async Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
        => _request.Create()           // 建立已 Config 過的 request
            .Then(SendAsync)           // 然已非同步非送出request
            .WaitThen(GetContentAsync) // 等待結果後取得Content
            .Then(LoadHtml)            // 最後讀取Html
            .Result;                   // 取得結果回傳
```

所以

**依照提供的Request定義以非同步方式取得Html物件序列**

的詳細流程就是

**`建立已 Config 過的 request，然後以非同步非送出request`**
**`等待結果後取得Content，最後讀取Html，取得結果回傳`**

這樣子程式本身就可以達到懂英文的皆可讀的程度

那中間要如何去完成呢？

先從第一個　Then開始，所以就這樣寫

```csharp
    public interface IChain
    {
        IChain Then(Func<HttpRequestMessage,Task<HttpResponseMessage>> request);
    }
```

如果我們希望他能夠持續被使用，就得使用泛型

```csharp
    public interface IChain<T>
    {
        IChain<TNext> Then<TNext>(Func<T,TNext> next);
    }

    public class Chain<T> : IChain<T>
    {
        private T _current;

        public Chain(T current) 
            => _current = current;

        IChain<TNext> Then<TNext>(Func<T,TNext> next)
            => new Chain<TNext>(next(_current));
    }
```

這樣子我們就可以一直 Then 下去了　以我們的爬蟲案例來

```csharp
    public async Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
        => _request.Create()
            // IChain<HttpRequestMessage>.Then<Task<HttpResponseMessage>>
            .Then(SendAsync)
            // IChain<Task<HttpResponseMessage>>.Then<Task<Task<Stream>>>
            .Then(GetContentAsync)
            // IChain<Task<Task<Stream>>>.Then<Task<Task<IEnumerable<HtmlElement>>>>
            .Then(LoadHtml)
            .Result;
```

看來案情並不單純，這個疊兩層的Task 可不是我們想要的東西

所以我們得把非同步時的情況用另一個方法來解決

```csharp
    public interface IChain<T>
    {
        IChain<TNext> Then<TNext>(Func<T,TNext> next);

        IChainAsync<TNext> WaitThen<TNext>(Func<T,Task<TNext>> next);

        T Result { get; }
    }

    public interface IChainAsync<T>
    {
        IChainAsync<TNext> Then<TNext>(Func<T,TNext> next);

        IChainAsync<TNext> WaitThen<TNext>(Func<T,Task<TNext>> next);

        Task<T> Result { get; }
    }
```

用這樣的方式就可以滿足我所需要的寫法了

```csharp
    public async Task<IEnumerable<HtmlElement>> GetAsync(Action<HttpRequestMessage> config)
        => _request.Create()
            // IChain<HttpRequestMessage>.WaitThen<Task<HttpResponseMessage>>
            .WaitThen(SendAsync)
            // IChainAsync<HttpResponseMessage>.WaitThen<Task<Stream>>
            .WaitThen(GetContentAsync)
            // IChainAsync<Stream>.Then<IEnumerable<HtmlElement>>
            .Then(LoadHtml)
            // Result = Task<IEnumerable<HtmlElement>>
            .Result;
```

介面我們大約已經完成了，至於實作的方法待下回分曉

我們先回到原主題，不知道對於這樣的一個做法

雖然他在思考方式，技巧運用等等都用了較為困難的方法

但是他最後出來的結果是淺顯易懂的

而一開始所說的"人類可讀，至少到目前也有個標的與雛形

## 備註
---

　- 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day6)
　- 關於TDD的概念(雖然只是借用)，請參考正式的TDD教學，
　- TDD推薦文章：在下也是看 [91大的文](https://dotblogs.com.tw/hatelove/2013/01/11/learning-tdd-in-30-days-catalog-and-reference) 長大的
　- 後續每篇最後會加上目前已經用過的一些個人對於Naming的小心得

## 命名小心得
---

　- Client vs Operation : Client 通常為可做出多項指令的東西，而 Operation 則是已經做出來的許多指令的介面
    　- 參考來源 : [Azure C# Sdk](https://github.com/Azure/azure-sdk-for-net/blob/psSdkJson6/src/SdkCommon/ClientRuntime/ClientRuntime/IServiceOperations.cs) ServiceClient<TService> vs IOperation<TService>
　- Loader vs Getter : Loader 往往需要做I/O的處理，而 Getter 只是普通的取出，更明確的概念 Loader 可能是一點一點地取出或接收，而 Getter 則是一次拿完
    　- 參考來源 : System.IO.File ...etc , and Class Property