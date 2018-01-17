## Day-30 一切的努力，都只是為了那唯一的目標
---

接續前篇最後問題，得把透過擴充方法帶入的方法參數也 Handle

要完成這件事情，就必須將擴充方法中的方法在經過處理之前就先完成 Handle

所以利用 interface 將```IChain<>```與```IChainAwaiter<>```加以擴充

讓擴充方法也可以使用 Handler 並且在 Handle 結束後在做轉換變成原本建立下一個 Chain 所需要的參數

```csharp

    public class FuncParser<T>
    {
        private readonly IHandler _handler;
        private readonly Action<Func<T>> _callback;

        internal FuncParser(IHandler handler,Action<Func<T>> callback)
        {
            _handler = handler;
            _callback = callback;
        }

        public void Parse<TLast>(TLast arg,Func<TLast, T> func)
            => _callback(Handle(func,arg));

        private Func<T> Handle<TLast>(Func<TLast, T> func,TLast arg)
        {
            _handler.Handle(func);
            return () => func(arg);
        }
    }

    public class TaskParser<T>
    {
        private readonly IHandler _handler;
        private readonly Action<Task<T>> _callback;

        internal TaskParser(IHandler handler,Action<Task<T>> callback)
        {
            _handler = handler;
            _callback = callback;
        }

        public void Parse<TLast>(TLast arg, Func<TLast, Task<T>> func)
        {
            _handler.Handle(func);
            _callback(func(arg));
        }

        public void Parse<TLast>(Task<TLast> arg, Func<TLast, T> func)
        {
            _handler.Handle(func);
            _callback(Task.Run(async () => func(await arg)));
        }

        public void Parse<TLast>(Task<TLast> arg, Func<TLast, Task<T>> func)
        {
            _handler.Handle(func);
            _callback(Task.Run(async () => await func(await arg)));
        }
    }

```

```Parser```的目的在於轉換方法參數成下一個 Chain 建立的建構參數

而其中的```Callback```的目的是因為這個方法因為是由擴充方法內呼叫

但希望取得結果的是```ChainFactory.Create```，所以透過這樣的Callback來將結果推送給它

所以ChainFactory會變成這樣

```csharp

    public class ChainFactory : IChainFactory
    {
        public IChain<T> Create<T>(Action<FuncParser<T>> config)
        {
            Func<T> factory = null;
            config(new FuncParser<T>(_handler,func => factory = func));
            return new LazyChain<T>(this, Handle(factory));
        }

        public IChainAwaiter<T> Create<T>(Action<TaskParser<T>> config)
        {
            Task<T> task = null;
            config(new TaskParser<T>(_handler,t => task = t));
            return new ChainAwaiter<T>(this, Handle(task));
        }
        //....其他省略
    }

```

這裡讓Parser在ChainFactory中建構而不是擴充方法的目的

是為了後續也許可以讓Parser也透過DI來做為建構方式

所以擴充方法也做相對應的調整

```csharp

    public static class FluentExtensions
    {
        public static IChain<TNext> Then<T, TNext>(
            this IChain<T> chain,
            Func<T, TNext> func)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, func));

        public static IChainAwaiter<TNext> Then<T, TNext>(
            this IChain<T> chain,
            Func<T, Task<TNext>> next)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, next));

        public static IChainAwaiter<TNext> Then<T, TNext>(
            this IChainAwaiter<T> chain,
            Func<T, Task<TNext>> next)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, next));

        public static IChainAwaiter<TNext> Then<T, TNext>(
            this IChainAwaiter<T> chain,
            Func<T, TNext> next)
            => chain.Factory.Create<TNext>(p => p.Parse(chain.Result, next));
    }

```

```Action<>```此時的目的則是用於將擴充方法中的兩個參數帶入的才實際建立```Parser```來套用的方法

利用建構 Parser 的時候帶入Callback的內容，當config方法參數被執行完成後，所需要的參數也已經Set完成

而這也其實就是把非同步全部黏起來寫成一排的情境

如此一來所有的功能都已經連接在一起了，最後我們來打造這些功能的使用法和入口

```csharp

    public class ResultContext
    {
        private readonly Func<object> _getter;

        public ResultContext(Func<object> getter)
        {
            _getter = getter;
        }

        public object Result => _getter();
        public Type Type => Result.GetType();
    }

    public class CallerContext
    {
        public CallerContext(object caller)
        {
            Target = caller;
        }
        public object Target { get; }
        public Type Type => Target.GetType();
        public Type From => Type.GenericTypeArguments.First();
        public Type To => Type.GenericTypeArguments.Last();
        public Delegate Delegate => (Delegate)Target;
        public MethodInfo CallerInfo => Delegate.Method;
        public string CallerName => CallerInfo.Name;
    }

```

```ResultContext```是用於Handle單一次結果的Context

```CallerContext```是用於Handle方法參數的Context

所以在把所有 Handle 方法的參數改為帶入這種強行別的類別

```csharp

    public class ChainFactory : IChainFactory
    {
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
        //其他省略....
    }

    public class FuncParser<T>
    {
        private Func<T> Handle<TLast>(Func<TLast, T> func,TLast arg)
        {
            _handler.Handle(new CallerContext(func));
            return () => func(arg);
        }
        //其他省略
    }
```

如此一來```IHandler.Handle<>()```方法就可以不需要漫無目的地去尋找對象

所以在註冊服務時，額外加上定義方法

```csharp

    public class ChainBuilder
    {
        public ChainBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

    public static class ServiceCollectionExtensions
    {
        public static ChainBuilder AddChain(this IServiceCollection services)
        {
            services.AddTransient<Chain>()
                .AddTransient<IChainFactory, ChainFactory>()
                .AddSingleton<IHandler, Handler>();
            return new ChainBuilder(services);
        }

        public static ChainBuilder AddResultHandler(
            this ChainBuilder builder,
            Action<ResultContext> handler)
        {
            builder.Services.AddSingleton<IHandler<ResultContext>>(new Handler<ResultContext>(handler));
            return builder;
        }
        public static ChainBuilder AddCallerHandler(
            this ChainBuilder builder,
            Action<CallerContext> handler)
        {
            builder.Services.AddSingleton<IHandler<CallerContext>>(new Handler<CallerContext>(handler));
            return builder;
        }
    }

```

這樣就大功告成了，而且修改前的測試也沒有任何錯誤，這樣的功能可以達到什麼樣的效果呢？

透過以下的設定，跑出來的結果大概是這樣

```csharp
var logwriter = new StringWriter();
new ServiceCollection()
    .AddChain()
    .AddCallerHandler(ctx => logwriter.WriteAsync($"use {ctx.From.Name} invoke {ctx.CallerName} will get {ctx.To.Name} "))
    .AddResultHandler(ctx => logwriter.WriteLineAsync($"is {ctx.Result}"))
//----中略並輸出 logwriter的字串如下
use TextWriter invoke WriteOne will get TextWriter is 1
use TextWriter invoke WriteTwoAsync will get Task`1 is 12
use TextWriter invoke WriteOne will get TextWriter is 121
use TextWriter invoke WriteTwoAsync will get Task`1 is 1212

```

這是我用TextWriter作為Chain的起始物件依序以兩個方法來交互執行後所得到的Log

雖然還有不少可以補強的部分，但是我們的目的也達到了

而最後，為什麼我們需要 Clean Code , 為何我們需要更清楚的命名規範

這樣的 Log 我想也有機會成為其中一個值得努力的理由吧

## 如此一來，我們將不再需要每次都去了解程式到底是如何運作的

## 程式會自己告訴我們它是如何運作的，不是嗎?

與其多說不如直接 Run 看看吧

## 備註：

---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day30)
 - 爬蟲應用Chain的整合測試會後續補上在上述的github中
 - 趁機推廣一下個人的[GitBook技術觀點與心得](https://dcvsling.gitbooks.io/tech-descript/)，本篇內容亦會在未來找時間擷取精華收錄於此書上
 - 這本書的原初目的是為了記錄我對於一項技術的了解與觀點做下的筆記，所以依照以往經歷，可能會有太過於抽象，但...我們寫的程式不就也很抽象嗎? XD
 - 個人非常鼓勵利用像是[GitBook](https://www.gitbook.com/)這樣的免費資源做為自己的技術技能樹，就像是我們利用單元測試來驗證程式是否符合預期，我們也可以利用這樣的資源來記錄我們驗證過的事情，還可以做技術知識交流傳承，觀念差異比對 ~~翻舊帳時的資源站~~

## 追加心得：以下已無任何技術內容，純屬心得和感受

---

很感謝您如此有毅力看到這裡，因為以我所接觸到的人中，這些內容可能有如古代魔法書一般的 Magic~~

前面說抽象也不是毫無根據的，如果這些內容您都有辦法看懂其內容並且已經融入你的開發習慣

那至少在我的觀點，你也是個厲害的強者，雖然我覺得在台灣似乎並不喜歡這種類型的開發者

如果您仍然懵懵懂懂地看到這裡，仍有許多疑惑，我想也許我大概也無法解釋的清楚

因為再怎麼解釋，還是那麼的抽象，~~ 飄~~~ ~~

不過能看到這裡的我相信都多少一定是技術愛好者或是對這些內容深感興趣的人

歡迎以任何方式交流，個人的交流觀點如下：

- 對事不對人不見得代表不會引發情緒問題，與基督徒談無神論，信念或信仰受到質疑時，這是在所難免的，所以對是不對人，只是在於當討論結束後，可以更容易釋懷，而且並不代表不可以對人不對事
- 永遠不要創造一場別人贏的遊戲，因為你永遠不會知道對方花了多少的努力去參與這場遊戲，而最後發現努力根本白費，雙方都毫無收穫，沒有人會想參與這樣的遊戲
- 不需要害怕去面對一場可能會很激烈的辯論，就像是下圍棋，場上激烈的廝殺，對亦者與旁觀者的思路，是截然不同的，收獲也是完全不同，如果真的要說差異，那就是因為事不關己，所以沒有感覺
- 某種程度上，把Code放在網路上給大家電一電的收穫，高過花錢去上課，所以面子折算台灣的學費可能還有點價值，但折算國外的可能快不值錢了
- 前篇最後那個影片的後續，Neo摔到地面這件事情，你有一起把它看完嗎？這是必定會發生的事情喔~~其實最後只是想講這個吧 XD~~