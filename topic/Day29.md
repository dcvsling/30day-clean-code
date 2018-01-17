## Day-29 Morpheus : Free Your Mind
---

在開始之前，先把一個熱門好一陣子的框架拿出來談談吧

### [Middleware](https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/middleware?tabs=aspnetcore2x)

詳細請參考上方連結的微軟官方說明文件，我一直覺的微軟現在的[MSDN Document](https://docs.microsoft.com/zh-tw/)做的非常的好

就算不是M$使用者也可以看一下拿回自己擅長的領域用亦可

這裡簡單介紹所需要關注的重點

藉由 Context 這種具有上下文特性的 DTO 來貫穿整個流程，並且依照 Context 最後的結果來做為結果值

以 Http Service 為例就是 HttpContext ，HttpContext 具有 Request 的資訊以及，Response 最後回傳的內容

並且由每一個經過的方法來做調整，所以每一個方法都可以處理這次的 Request 當然也可以阻斷流程進行

在 .Net Core中，每一個流程都是一個 delegate 叫做　RequestDelegate

```csharp

public delegate Task RequestDelegate(HttpContext context);

```

所以說，如果有```HttpContext```而且還有```RequestDelegate```，那就可以執行該委派

```csharp

public Task ProcessRequest(HttpContext context, RequestDelegate request)
    => request(context);

```

這其實就是 Owin Middleware 的原型，但這樣還不夠，這樣必須等到執行階段才能開始組合

所以這次借用科學上一個很常用的做法叫做[假說]()

簡單來講就是假裝已經有了，~~先自我催眠一下(誤)~~

所以就假裝已經有```HttpContext```存在的情況下

```csharp

private HttpContext _context;

pubilc Task Request(RequestDelegate request)
    => ProcessRequest(_context,request);

```

如此一來這整個程式最終還是會變回```Func<HttpContext,Task>```也就是```RequestDelegate```

所以最後演變成

```csharp

public RequestDelegate Middleware(RequestDelegate request)
    => ctx => request(ctx);

```

而這樣有什麼好處呢？這表示可以這樣做

```csharp

public RequestDelegate Middleware(RequestDelegate request)
    => async ctx => {
        // do anything you wait before process request
        await request(ctx);
        // do anything you wait after process request
    };

```

而且這樣做完之後往後傳遞並不會有型別上的不同，後續的人一樣可以做相同的事情

這次就來利用這樣的特性來完成所需要的功能吧

於前篇所做的``ChainFactory```中，做那樣的調整的目的就是為了這件事情

```csharp

    public class ChainFactory : IChainFactory
    {
        private readonly IHandler _handler;

        public ChainFactory(IHandler handler)
        {
            _handler = handler;
        }

        public IChain<T> Create<T>(Func<T> func)
        {
            return new LazyChain<T>(Handle(factory));
        }

        public IChainAwaiter<T> Create<T>(Task<T> task)
        {
            return new ChainAwaiter<T>(Handle(task));
        }

        private Func<T> Handle<T>(Func<T> factory)
        {
            var result = factory();
            _handler.Handle(new ResultContext(() => result));
            return () => result;
        }

        private Task<T> Handle<T>(Task<T> task)
        {
            _handler.Handle(new ResultContext(() => task.Result));
            return task;
        }
    }

```

如此一來就可以將```IHandler```接上接下來要使用的對象

這裡要注意的是，```Func<>```與```Task<>```特性不同點

- ```Func<>```：每一次執行都會重做一次全部的過程，所以當取得一次結果之後，將結果重新包裹回傳
- ```Task<>```：當執行完畢後，每一次 await 都會取得完全相同的物件，所以利於後續撰寫則回傳 Task

```ChainFactory.Handle```這個方法就如同前述的```Middleware```的結構

所以不管此處我們如何包裹原本的物件，總之回傳一樣的就行了

而此處只有在對於取得的結果做 Handle，但對於前面的參數方法沒有 Handle 到

所以接下來會在要連同外面複雜的擴充方法問題一併解決掉

## 備註：

---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - Middleware 的用法在近期的MVC Framework中是非常常見的
 - 此框架原出處似乎為 Ruby 但現在 .Net , PhP 的MVC 幾乎都是這個框架
 - 解決方案並不受語言限制，使用方法也不受問題限制
 - [Free Your Mind](https://www.youtube.com/watch?v=gmawiQsMJPc&t=144)