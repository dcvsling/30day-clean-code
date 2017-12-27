## Day-9 失敗不代表結束，而是另一個新的開始
---

前篇最後提到，用泛行來解決最後的相同參數問題

為了避免編譯器優先選擇原始類別或介面去執行

所以考量將所有的方法移出

只留下Result，然後將方法都移出變成擴充方法，並且調整其泛型成為為我期望的樣子

```csharp
    public class Chain<T> : IChain<T>
    {
        private T _current;
        internal Chain(T current)
        {
            _current = current;
        }
        
        public T Result => _current;
    }

    public static class ChainHelper
    {
        public static IChain<TNext> Then<T,TNext>(this IChain<T> chain,Func<T, TNext> next)
            => new Chain<TNext>(next(chain.Result));

        public static IChainAwaiter<TNext> Then<T,TTask,TNext>(this IChain<T> chain,Func<T, TTask> next)
            TTask : Task<TNext>
            => new ChainAwaiter<TNext>(next(chain.Result));
    }
```

而實際上回到Then時發現其型別似乎並未如我所預期的去轉換，實際編譯器顯示如下
```csharp
IChain<Task<HttpResponseMessage>> IChain<HttpRequestMessage>.Then<HttpRequestMessage, Task<HttpResponseMessage>>(Func<HttpRequestMessage, Task<HttpResponseMessage>> next)
```
我期望他會走回傳```IChainAwaiter<HttpResponseMessage>```的那方法，但經過幾番嘗試後發現

在我預期中泛型似乎無法作到此效果，(編譯器表示：你以為換個名字就能通過我的法眼嗎~~XD)

hmm...........................................................................

不如來試試看前篇提到的另一件事情

也就是 namespace 的作法好了

**`接下來所作的僅為實現其目的，而也許在現實中我不會去實踐接下來的程式，所以請斟酌使用`**

從namepsace 中我們可以知道這是一個組件的路徑

而實際上我們可以在namespace的內外各自宣告 Using namespace 像是這樣

```csharp
using Fluent.Sync;
namespace Ithome.IronMan.Example
{
    using Fluent.Async;
}
```

所以首先將我們所寫的Chain系列類別全部都先換個名字叫做Fluent的　namespace

再來我們將剛剛都移出來的擴充方法分成兩份放在兩個不同的namespace

分別是下一個回傳為非同步的情境

```csharp
namespace Fluent.Async
{

    public static class ChainHelper
    {
        public static IChainAwaiter<TNext> Then<T,TNext>(this IChain<T> chain,Func<T, Task<TNext>> next)
            => new ChainAwaiter<TNext>(next(chain.Result));

        public static IChainAwaiter<TNext> Then<T,TNext>(this IChainAwaiter<T> chain,Func<T, Task<TNext>> next)
            => new ChainAwaiter<TNext>(chain.Result.ContinueWith(async t => await next(await t)).ContinueWith(t => t.Result.Result));
    }
}

```

以及 下一個回傳唯一般的情境

```csharp
namespace Fluent.Sync
{
    public static class ChainHelper
    {
        public static IChain<TNext> Then<T,TNext>(this IChain<T> chain,Func<T, TNext> next)
            => new Chain<TNext>(next(chain.Result));

        public static IChainAwaiter<TNext> Then<T,TNext>(this IChainAwaiter<T> chain,Func<T,TNext> next)
            => new ChainAwaiter<TNext>(chain.Result.ContinueWith(t => next(t.Result)));
    }
}
```

這時後再回到 NewCrawler.cs 上發現，錯誤居然都消失了

所有的型別都按照原本的期望去轉變

這是什麼巫術 (笑)

這裡所運用的概念是，在裡面的namespace 會被優先選擇作為重複方法的首選

這種做法有時在我們自己開發的元件與其他人，或官方開發的元件名稱重複而且也不想去改變作法的時候

可以選擇將其中一個組件一道namespac之外，這樣就不會有名稱重疊問題

也不會造成因為沒有Using而導致其中一邊的其他處使用的功能失效

這裡提供官方的[命名空間](https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/namespaces/)作輔助

因此，我只要把我希望他優先作考量的型別在裡面作Using 那他就能準確的執行我所期望的方法了

## 備註
---

　- 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day9)
　- 我可能不會這樣去當作最後的程式碼的理由是，利用namespace 的作法，其邏輯隱密性太高，除非團隊都可以接受，否則不如從方法名稱上下手吧
　- 此路不通不代表完全無解，而許許多多的方法都是人想的，**沒有想不到，只有沒想到**

## 命名小心得
---

  - Extensions vs Helper : Extensions 用來對外表示對於指定型別的擴充，而Helper 往往會用於內部功能

  - Sync vs Async : 非同步方法(回傳值為Task)的方法名稱最後須加上Async 已作為此方法為非同步方法