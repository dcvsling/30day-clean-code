## Day-3 能不能用一句話說完!?
---

把上回最後寫好的程式的註解搬出來看一下
```
  類別名稱 : 爬蟲
  方法名稱 : 依照參數網址開始運行並回傳結果
  詳細內容 : 
    1.建立Httpclient物件 
    2.用client以非同步 GET 的方式 Request指定網址取得回應並等待結果
    3.將回應的Content再以非同步的方式讀取內容串流並等待結果
    4.建立HtmlDocument物件讀取剛剛取出的串流回傳HtmlDocumentNode中所有Element名稱為"a"的Element
    5.放入List中回傳
```
從這裡我們可以看到幾件事情：
  1. 是否建立HttpClient是爬蟲該做的事情?
  1. Request是以非同步進行，所以是否在等待時不要占用資源?
  1. 從拿到Response之後是交由另一個套件來解析，而中間還有一個轉送Stream 的功能，這件事情是否足夠重要呈現在這個地方來描述?
  1. HtmlDocument與HtmlNode 是爬蟲所使用的第三方套件，直接將第三方套件型別傳送出去會造成強迫使用者直接相依於此套件
  1. 有的網頁頗大的，直接把結果放進List中，可能會造成記憶體問題
  1. 這是爬蟲，所以除了這些外往往還會有像是批次，排程，自動化等等
  1. 這是爬蟲(很重要所以要說第二次)，在網路發達的年代泛用性太廣了，所以建立一個高泛用性的爬蟲，也許可以對未來有很大的幫助

而如果我們套用目前常聽到的幾個開發原則的話，可以在看出不少問題

在Clean Code 的術語中，也就是Bad Smell 

這些問題會在後續講述的過程中一一解決

而這裡我們目前關注的重點就是，功能性差之外

##這隻爬蟲做太多事情了，所以看起來很複雜

那有簡單的描述嗎?

Ex: 依照提供的Request定義以非同步方式取得Html物件序列

翻譯成程式碼就是

```
public interface ICrawler
{
    /// <summary>
    /// 依照提供的Request定義 => Action<HttpRequestMessage>
    /// 以非同步方式取得 　　　=> GetAsync
    /// Html物件序列          => IEnumerable<HtmlElement>
    /// </summary>
    /// <param name="config">Action[HttpRequestMessage]</param>
    /// <returns>IEnumerable[HtmlElement]</returns>
    IEnumerable<HtmlElement> GetAsync(Action<HttpRequestMessage> config);
}
```
也許可能會覺得　interface 本來就不會描述細節

而的確是這樣，爬蟲的細節本來就不需要在一開始就被決定

而這就是其中一個，interface 很好用的方式

下一次會講這樣的做法可以帶來哪些好處

備註
---
　- 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day3)
　- 類別 [HtmlElement](https://github.com/dcvsling/30day-clean-code/blob/Day2/src/HtmlElement.cs)請參考隨附的範例