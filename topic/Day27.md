## Day-27 如空氣一般的 Log
---

以前跟朋友討論過，什麼樣的 Log 形式是最好的呢？

而我們當時的結論是

## 不用寫在我程式碼裡面的Log是最好的

其中一種方法就是，讓你的程式寫得不需要 Log ~~就是本篇的主軸啦~~

而另一種方式就是，前面提到的注入

基本上就算寫得在美在好，Log還是要有的

現在網路上各式各樣的 Logging 機制，還可以用REC的只將錯誤給錄下來

但這次沒有要講那種，但關於前面提過的注入這件事情

其實準備工作已經完成了，也就是利用```Chain<>```來完成我們的目的

而就像前面說的，修理問題之外，還要順便讓她更好

這個看似棘手的，型別總是對不準的玩意就先把他從 Crawler Project 上搬出來吧

創建一個 Project 叫做 ```Ithome.IronMan.Example.Plugins```~~或是你可以自己選名字 XD~~

然後把原本的 ```Chain<>```，以外的都搬過來

現在要做的就是抽換他的實作，並且介紹一個常見的用詞叫做

## Lazy 

Lazy 的意思就是，有需要才做，所以才叫做懶

像是 Lazy Loading 就是有需要才讀取，Lazy<T> 就是有需要才建立

而這裡剛好可以與 Func<T> 與　Task<T> 一起來比較一下

| State     | Func\<T\>        | Lazy\<T\>          | Task\<T\>           |
|-----------|------------------|--------------------|---------------------|
| on Define | create func      | create lazy        | create task and run |
| on Access |   N\A            | create T and Get T | wait and Get T      |
| on Invoke | invoke and get T | N\A                | N\A                 |

所以在某種程度上，這三個型別的行為是很相近的

而這一點剛好可以拿來好好利用一下

在開始動刀之前，別忘了先用測試輔助我們的調整 ~~這裡簡單實作一個示意~~

```csharp
    [Fact]
    async public Task ChainInvokeThen_OneTwoTwice_WillGet1212()
    {
        TextWriter writer = new StringWriter();
        var expect = "1212";

        var result = await Chain.StartBy(writer)
            .Then(WriteOne)
            .Then(WriteTwoAsync)
            .Then(WriteOne)
            .Then(WriteTwoAsync)
            .Result;
        var actual = result.ToString();

        Assert.Equal(expect, actual);
    }

    private TextWriter WriteOne(TextWriter writer)
    {
        writer.Write("1");
        return writer;
    }
    async private Task<TextWriter> WriteTwoAsync(TextWriter writer)
    {
        await writer.WriteAsync("2");
        return writer;
    }
```

接下來建立新的 Chain 叫做 ```LazyChain<>```

```csharp
    public class LazyChain<T> : IChain<T>
    {
        private readonly Func<T> _func;
        private T _current;
        public LazyChain(T current) : this(factory,() => current)
        {
            _current = current;
        }

        public LazyChain(Func<T> func)
        {
            _func = func;
            Factory = factory;
        }

        public T Result => _func();
    }
```

為何是```Lazy```呢？因為他只有在 Result 被 Access 時才會真的開始去取得 T

而為何要這樣用的原因是為了縮短 T 與 Task\<T\> 的差異

改用較為相近的 Func<T> 來代替

如此一來可以徹底化解當初我們所遇到的困境 ~~所以說，還是有解的~~

```csharp
    public static class FluentExtensions
    {
        public static IChain<TNext> Then<T, TNext>(this IChain<T> chain, Func<T, TNext> func)
            => new LazyChain(() => func(chain.Result));

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChain<T> chain, Func<T, Task<TNext>> next)
            => new ChainAwaiter(next(chain.Result));

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChainAwaiter<T> chain, Func<T, Task<TNext>> next)
            => new ChainAwaiter(chain.Result.ContinueWith(async t => await next(await t)).ContinueWith(t => t.Result.Result));

        public static IChainAwaiter<TNext> Then<T, TNext>(this IChainAwaiter<T> chain, Func<T, TNext> next)
            => new ChainAwaiter(chain.Result.ContinueWith(async t => next(await t)).ContinueWith(t => t.Result.Result));
    }

    public abstract class Chain
    {
        public static IChain<T> StartBy<T>(T current)
            => new LazyChain<T>(current);

        public static IChainAwaiter<T> StartBy<T>(Task<T> task)
            => new ChainAwaiter<T>(task);
    }
```

## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 當初想破頭無解的事情，並不需要那麼早的斷言他不可行
 - 一個方法可不可行並不是取決於自己或他人是否能做到，而是存不存在互斥的條件
   - 封閉內網想要使用外網的服務，中間無法通過屬於互斥的條件
   - 一個方法依照參數回傳不同的結果，似乎只有範圍內互斥而已
 - 把 T 變成 Func\<T\>就能解決問題的理由是 T 可以為 Task\<T\> ，但Func\<T\> 就不會是 Task\<T\>
   - 推斷因素好難解釋，等我想到比上面這個理由更好的答案時再來討論吧