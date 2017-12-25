# Day-7 既然不同步，那怎麼說就怎麼做囉

---

接續前篇說的介面如下

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

這裡運用到了 Task 與 Func<>

我先來簡單介紹一下 Task 與 Func 給不熟悉的人

首先 Func<> 與 Action極其多個泛行型別 皆屬於委派

而Action 是無回傳值委派，其泛型型別參數皆屬於該委派參數型別

而Func<>則是具回傳值委派，其泛型參數至少一存在一個為回傳型別參數並放在最後一位，其餘皆屬於委派參數型別

而 Task 則是與這兩個東西息息相關

Task 主要分為　Task 與　Task<> 現階段較常見的都用 Task.Run 來建立

而建立方法如果帶入 Action 則會回傳　Task

如果帶入　Func<> 則會回傳　Task<>

而如果簡單描述　Func<> 與　Task<> 的差別的話

Func<> 是**執行**當下開始動作，然後等結果

Task<> 則是**建立**當下就開始動作，然等結果有了之後取結果

如果把他們合再一起就會變成

Task<Func<>> : 這其實沒什麼意思，因為 Task 本身就是用Func建立起的

Func<Task<>> ： 這個就很有趣了，這就成了一個延遲Task 開始運行並且可由使用端來決定開始時間的微妙結構

我們延續之前的步驟，依序說明每一段個代表的意思所以用上什麼樣的實作

```csharp
    /// <summary>
    ///　非同步的方法責任練
    /// </summary>
    public class Chain<T> : IChain<T>
    {
        private T _current;
        internal Chain(T current)
        {
            _current = current;
        }
        /// <summary>
        /// 接著走下一步到下一個階段
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>下一個階段</returns>
        public IChain<TNext> Then<TNext>(Func<T, TNext> next)
            => new Chain<TNext>(GetNextValue<TNext>(next));

        /// <summary>
        /// 接著走下一步到下一個非同步階段
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>分同步階段</returns>
        public IChainAsync<TNext> ThenAsync<TNext>(Func<T, Task<TNext>> next)
            => new ChainAsync<TNext>(Task.Run(() => next(_current)));

        /// <summary>
        /// 取得下一個階段的回傳值
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>回傳值</returns>
        private TNext GetNextValue<TNext>(Func<T,TNext> next)
            => next(_current);

        /// <summary>
        /// 取得非同步的下一個階段的回傳值
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>非同步的回傳值</returns>
        private Task<TNext> GetNextValueAsync<TNext>(Func<T, Task<TNext>> next)
            => next(_current);
        public T Result => _current;
    }
```

```csharp
    /// <summary>
    /// 非同步的方法責任練
    /// </summary>
    public class ChainAsync<T> : IChainAsync<T>
    {
        private Task<T> _task;
        internal ChainAsync(Task<T> task)
        {
            _task = task;
        }

        /// <summary>
        /// 用等待的結果，接著走下一步到下一個非同步階段
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public IChainAsync<TNext> WaitThen<TNext>(Func<T,Task<TNext>> next)
            => new ChainAsync<TNext>(GetNextValueAsync<TNext>(next));

        /// <summary>
        /// 用等待的結果，接著走下一步到下一個階段
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public IChainAsync<TNext> Then<TNext>(Func<T,TNext> next)
            => new ChainAsync<TNext>(GetNextValue<TNext>(next));

        /// <summary>
        /// 取得下一個結果
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        async private Task<TNext> GetNextValue<TNext>(Func<T,TNext> next)
            => next(await _task);

        /// <summary>
        /// 取得下一個非同步的結果
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        async private Task<TNext> GetNextValueAsync<TNext>(Func<T, Task<TNext>> next)
            => await next(await _task);
            
        public Task<T> Result => _task;
    }
```

直至目前我們已經近乎要達成我們原本期望的情況

問題還差在同步方法串中不同存在相同名稱不同回傳值的方法

也就是說我們不能在同一個類別內同時寫

```csharp
IChain<TNext> Then<TNext>(Func<T,TNext> next);

IChainAsync<TNext> Then<TNext>(Func<T,Task<TNext>> next);
```

不過這是小事，下一次就來完成我所期望的描述法吧．

## 備註
---

　- 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day7)
　- 在下這次偷偷廣告我個人對於 [Task的觀點](https://dcvsling.gitbooks.io/tech-descript/content/CSharp/asyncawait.html)

## 命名小心得
---

  - Builder vs Factory : Builder 通常可以提供我們Build的操作設定，而Factory則只能提供簡單標籤
    - 範例 : 一個可以自己決定烹煮流程與時間的咖啡機叫做 Builder , 一件完成所需要的咖啡的咖啡機叫Factory

  - Configure vs Convention : Configure 通常來自於外部定義，Convention則為通常為內部預設定義
    - 例如 :你從外部介入調整咖啡機烹煮流程與時間較做Config，如果不介入調整咖啡機使用自己預定的流程則為Convention