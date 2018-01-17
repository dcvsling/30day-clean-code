## Day-28 想要做什麼東西，蓋工廠就對了
---

接著設計兩個 Logging 與 Error

基於以上兩個都屬於需要 Handle 的功能，所以這裡宣告一個```IHandler```即可

```csharp
    public interface IHandler<T>
    {
        void Handle(Action<T> handler);
    }
```

用法大概像是這樣

``` csharp

    public class Handler<T> : IHandler<T>
    {
        private readonly Action<T> _handler;

        public Handler(Action<T> handler)
        {
            _handler = handler;
        }

        public void Handle(T context)
            => _handler(context);
    }

```

這樣似乎不夠，還需要一個可以取得這些功能的方法

```System.IServiceProvider```是透過 Type 提供指定服務物件的內建interface

所以先透過它來取用所需要的指定Handler類型

```csharp

    public interface IHandler
    {
        void Handle<T>(T context);
    }

    public class Handler : IHandler
    {
        private readonly IServiceProvider _provider;

        public Handler(IServiceProvider provider)
        {
            _provider = provider;
        }
        public void Handle<T>(T context)
            => _provider.GetService<IHandler<T>>()
                .Each(handler => handler.Handle(context));
    }

```

因為需要將```IHandler```注入到```Chain```中，所以需要一個建立```Chain```的方法

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
            return new LazyChain<T>(factory);
        }

        public IChainAwaiter<T> Create<T>(Task<T> task)
        {
            return new ChainAwaiter<T>(task);
        }
    }

```

當然作為起始方法的類別也得轉為普通可建構的物件才能注入```IHandler```


```csharp

    public class Chain
    {
        private readonly IChainFactory _factory;

        public Chain(IChainFactory factory)
        {
            _factory = factory;
        }

        public IChain<T> StartBy<T>(T current)
            => new LazyChain<T>(_factory,current);

        public IChainAwaiter<T> StartBy<T>(Task<T> task)
            => new ChainAwaiter<T>(_factory,task);
    }

```

最後這次就偷懶仰賴一下 DI 來幫我們把剩下的做完吧

這次以 Microsoft.Extensions.DependencyInjection 作為 DI Container

同場佳映 Crawler 也放進去吧

```csharp

    public static class ServiceCollectionExtensions
    {
        public static ChainBuilder AddChain(this IServiceCollection services)
        {
            services.AddTransient<Chain>()
                .AddTransient<IChainFactory, ChainFactory>()
                .AddSingleton<IHandler, Handler>();
            return new ChainBuilder(services);
        }

        public static IServiceCollection AddCrawler(this IServiceCollection services)
            => services.AddChain()
                .AddTransient<IHttpClient,DefaultHttpClient>()
                .AddTransient<IHtmlLoader, DefaultHtmlLoader>()
                .AddTransient<ICrawler, Crawler>();
    }

```

如此一來，注入的準備就可以算是準備完成

上述的程式目的在於，將我們期望的 Handler 能夠注入 Chain 的功能內

再透過 Chain 來執行　Handler 已達成目的

## 備註：

---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 在.Net Core 上的模組化已經可以單純用 AddChain or AddCrawler 來完成
 - 這也是為什麼在.Net Core上可以 AddMvc 的理由，當然啦，實際內容不會像上面那樣單純
 - 透過同樣的設定方式做為開發的準則，我個人是覺得可以有效幫助程式的質感和泛用性