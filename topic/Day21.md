## Day-21 案情並不單純?
---

在看完Input 之後，我們來接著看看Output吧

基本上無特殊情況下，普通回應即可

所以，現在談得當然就是案情並不單純

這次的範例就屬於其中一種案例

案例：
- 於 Http 上我們不一定總是應該要把 status code 500 就 throw
- 可能對於 Response 甚至 header 需要有不同類型的回應

一般常見的 solution 是用 Result class 來做為解決問題的方法

ex：[IActionResult](https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Abstractions/IActionResult.cs)

此介面作為Mvc Controller的 Response 以回應其正確

而我們所使用的 View(),Json(),Ok() ...etc

其實都是實作了 IActionResult 這個介面

而這個介面裡面只有一個方法 

```csharp
Task ExecuteResultAsync(ActionContext context);
```

有沒有一種似曾相識的感覺，這其實就是 [RequestDelegate](https://github.com/aspnet/HttpAbstractions/blob/dev/src/Microsoft.AspNetCore.Http.Abstractions/RequestDelegate.cs)

所以 Mvc 透過IActionResult 將我們所帶入的各種不同的結果

以統一的方法來作為 Response 的結果

而實際上我們也並不需要非得引用 Mvc 才能做出這種效果

我舉一個簡單的例子

```csharp
public interface IResult
{
    Object Result { get; }
    Exception Exception { get; }
    IsSuccess { get; }
}

```

這個介面上就很清楚的表示了一個 Result 物件，一個例外以及確認是否成功

而這樣的 Result 在使用上通常會類似這樣

```csharp

public static class ResultExtensions
{
    public static object EnsureSucceed(this IResult result)
        => result.IsSuccess ? result : throw result.Exception;

    public static T EnsureSucceed<T>(this IResult result)
        => result.IsSuccess && result.Result is T resultT ? resultT : throw result.Exception;
}

result.EnsureSucceed().ThenMethod();
```

這樣已經算是清楚的了，更常見的反而可能是 ~~這裡取可預期最糟情境~~

```csharp
 
if(result.IsSucceed)
{
    try{
        RunProcessWithResult(result.Result as MyType);
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
else
{
    throw result.Exception;
}

```

而這樣的程式碼不但難以維護，可讀性也變差了

有時候程式碼寫的很糟糕並不完全是當下開發者的責任

所以我們來定義一下這次問題的總需求吧

- 可對應於各種回 傳case 的 result
- 同時也要可以提供Exception
- 必且可以規範使用者如何使用，並使其不會自發或慣性的產生[bad smell](https://en.wikipedia.org/wiki/Code_smell)
- ~~我想要通用版本~~
- ~~是的!我是PM而且是奧客~~ (註2)

.........

看來得好好重新設計一下這個 Result

## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - [自創的PM梗](https://github.com/dcvsling/30day-clean-code/blob/Day21/topic/Day18.md)
 - 利用程式碼來影響使用者的開發習慣這件事情是我開始使用[.Net Core](https://docs.microsoft.com/zh-tw/aspnet/core/getting-started) 時發現的
 - 現階段的封裝及模組化技術其實已經非常的強大，任何功能內容與行為皆可以設定檔之類的方式作為定義來源建構整個系統 ex: [OrchardCMS](https://github.com/OrchardCMS)