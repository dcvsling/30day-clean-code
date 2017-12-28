## Day-10 所以那個測試呢?
---

說的那麼厲害，所以那個測(ㄐㄧㄤˋ)試(ㄓ)呢？

我們就從爬蟲那個介面開始吧

這裡使用先前的方法

利用方法名稱來描述方法

就用 3A 來嘗試吧

Arrange :
 - CreateCrawler    建立爬蟲
 - OkHandler        會回傳的 Status Code 200 的HttpMessageHandler
 - ConfigureRequest 建立request
 - WithContent      包含Content
 - LINK_CONTENT     超連結的Content

Act : 前面幾篇已經描述很多，故略過

Assert:
 - Assert       斷言
 - Collection   集合
 - result       結果
 - HasHyperLink 有超連結

所以我們的測試方法名稱為

GetElement FromOkHandler IsHyperLink

取得元素 從OKHandler 有超連結

所以最後寫成下面的樣子(其他方法名稱或變數請參考[範例]((https://github.com/dcvsling/30day-clean-code/blob/Day10/src/CrawlerTests.cs)))

```csharp
    [Fact]
    async public Task GetElement_FromOkHandler_HasHyperLink()
    {
        var crawler = CreateCrawler<OkHandler>();
        var request = ConfigureRequest(WithContent(LINK_CONTENT));

        var result = await crawler.GetAsync(request);

        Assert.Collection(result,HasHyperLink);
    }
```

再用一樣的方法建立第二個測試吧

```csharp
    [Fact]
    async public Task GetElement_FromOkHandler_HasHyperLinkAndDiv()
    {
        var crawler = CreateCrawler<OkHandler>();
        var request = ConfigureRequest(WithContent(LINK_CONTENT,DIV_CONTENT));

        var result = await crawler.GetAsync(request);

        Assert.Collection(result, HasHyperLink, HasDiv);
    }
```

這次是 GetElement FromOkHandler HasHyperLinkAndDiv

所以這次的 request 是 ConfigureRequest WithContent CONTENT and DIV_CONTENT;

而最後為 Assert Collection result HasHyperLink and HasDiv

如此一來測試也一樣清楚可見


## 備註
---

　- 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day10)

## 命名小心得
---

  - Executor vs Invoker : Executor 像是所有東西皆準備好，等待執行指令的方法，Invoker則是提供呼叫的方法

  - Handler vs Callback : Handler 會處理指定的事情，Callback則是等待被呼叫後開始處理事情 