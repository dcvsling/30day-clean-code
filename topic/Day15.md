## Day-15 多國語言?
---

於前篇提了方法的 Input

現在來講 OutPut

Output 可就不像是Input 那麼單純了

就從最基本的開始吧

```object Method(object input)``` 一來一往一進一出，是正拳

  這就不評論優缺點了，正拳就是努力練就會越變越好，而且每天都在用

```IEnumerable<Object> Method(object input)``` 回傳序列是很重要的 ~~不是集合喔~~

  一班來說在c#一組物件要在方法中穿梭都盡量會使用IEnumerable<>而不是Array or List
   
  基於它的作用方式可避免消耗過大與運作加速 (註2)
  
  在C#上近乎無敵的做法，只是他碰上Task的時候還是會可能會讓很多人苦手(註3)
  
   ~~名稱太長要寫很久，而且很難記~~

```Action<T> Method(object input)``` 呼叫傳送門的概念，帶入素材，執行方法，獲得一個任(泛)意(型)門(Entry)

  其實這樣用的情境還滿少的
  
  通常這表示一個設定過程或是委派本身會成為主要傳遞型別
  
  但其實最不願意這樣寫的理由還是因為這等於是要使用者寫```Method(obj)(other)```
  
   ~~挖坑給別人跳的概念!?~~

```Func<T> Method(object input)``` 建造工廠? Build a Factory

  回傳Func<>其實算是常用的寫法
  
  例如像是 Builder<> Build Factory<> 然後 Factory<> Create 目標
  
  Factory就是Func<>所以這是一種Pattern

```ResultObject Method(object input,out object output)``` 我自己倒是真的不喜歡這樣寫

  這種狀況多半都類似需要回傳兩種不同型別的物件時使用
  
  例如 ```bool TryParse(object obj,out T val)```但是這樣一寫兩邊都造成了困擾
  
  所以後來在 C# 7的時候前後都有了改善
  
  Input方可以寫 ```Method(input,out var output) ? output : other``` 這是[Pattern Matching](https://docs.microsoft.com/zh-tw/dotnet/csharp/pattern-matching#the-is-type-pattern-expression) 
  
  而Output方也可以以最簡單的方式改成```(bool,T) Method(object input)``` 這是[```ValueTuple```](https://docs.microsoft.com/zh-tw/dotnet/csharp/deconstruct)
  
  但我會更喜歡接下來這個做法

```void Method(T object, Action<object> callback)``` 有一種濃濃的Reactive味道

  這種做法的概念就是，有事再叫我
  
  這樣寫的好處很多，而且有非常多經典的模式都是長這樣
  
  例如 Castle Dynamic Proxy，Interceptor Pattern，Owin Middleware
  
  不過高自由的就代表著並不好懂

  簡單舉例其中一種用法，利用這種作法搭配Task，在Owin中比較常見的Middleware 

```csharp
  async Task Invoke(HttpContext context,Func<HttpContext,Task> next)
  {
     //before next....
     awawit next(context);
     //after next.....
  }
```

```IDisposable Method<T>(Action<T> next,Action<Exception> error,Action complete)``` 就是這個味

  上面都提了，那當然不能錯過這個狠腳色，他回傳的可不只是一個參數
  
  他回傳的是一組完整的try/cache/finally + disposer

  想像一下這結構大概像是這樣，順便幫這位高手正名一下

```csharp
   public class Observable : IObservable,IDisposable
   {
      private IObserver _observer;
      
      public Observable(IObserver observer)
      {
          this._observer = observer;
      } 

      public void Run(object obj) 
      {
          try
          {
              _observer.Next(obj);
          }
          cache(Exception ex)
          {
              _observer.Error(ex);
          }
          finally
          {
              _observer.Complete();
          }
      }

      public IDisposable Subscribe<T>(IObserver observer)
        => new Observable(observer);

      public void Dispose()
      {
          (_observer is IDisposable disposer
             ? disposer.Dispose
             : Empty.Action)();

          _observer = default;
      } 
   }

```

  這就是前面所說的，他不只給你一個回傳值

  他給你的是一整個架構完整的結構

```void Method<T>(T target) where T : class```這不是跑錯鵬唷
   
  這其實只是借用了 class 的　Reference特性在方法運作完之後繼續往後使用而不是等待回傳，這樣做可以確保物件不會被換過ex:[IConfigureOptions<>](https://github.com/aspnet/Options/blob/dev/src/Microsoft.Extensions.Options/IConfigureOptions.cs)，所以相較之下第一種方法就會是可以被置換的作法ex:[ModelBuilder](https://github.com/aspnet/EntityFrameworkCore/blob/dev/src/EFCore/ModelBuilder.cs)


看了那麼多種回應的方式，前一篇那種Input真的接得起來嗎？

就讓我們繼續看下去吧


## 備註：
---

  - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
  - 註2：過多的消耗是指這兩件事情
    1. 記憶體內存消耗，每一次做ToArray,ToList,....etc時，則會留存一次結果，當資料量過大時，就會很容易造成OutOfMemoryException [附上測試範例](https://github.com/dcvsling/30day-clean-code/blob/Day15/src/LinqTests.cs)
    2. 運算時間消耗，在Linq上像是First,Any,這樣的方法實際上並不會跑完整個迴圈，所以這也會比每一個迴圈都跑要的快速
  - 註3：Linq因為[延遲執行](https://docs.microsoft.com/zh-tw/dotnet/csharp/iterators)的緣故，所以她的```Task.Run``` 實際開始運作的時間點會被再延後，而同樣的```Func<Task>```也是同等案例 [附上官網警示](https://docs.microsoft.com/zh-tw/dotnet/csharp/async#important-info-and-advice)
  - IObservable/IObserver 並不表示　[Reactive extensions](https://github.com/Reactive-Extensions) 更不代表　[Reactive Programming](https://en.wikipedia.org/wiki/Reactive_programming)，這對好兄弟早在這些東西出現之前就存在了
  