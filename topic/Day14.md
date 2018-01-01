# Day-14 程式因為不想當邊緣人，所以.....
---

在語言的文法上，有所謂的[介系詞](http://www.taiwantestcentral.com/Grammar/Title.aspx?ID=6)

其目的在於讓前述的事物可以與其他詞句連貫，並表達其作用關係

所以我們在描述需求或是程式的時候

勢必也需要同樣的作法

通常這樣的文字並不適合放在前兩篇所描述的行為與事物

~~不然也不會特別叫介系詞了~~

例如像是我們前面所做的範例

```Crawler``` 中有 ```HttpClient```與　```HtmlLoader```

而其前後又還有　```Action<HttpRequestMessage>``` 與 ```IEnumerable<HtmlElement>```

所以為了將這幾個內容依序連貫起來，所以用了```Chain<>```和```ChainAwaiter<>```

而這次要來說的就是 ```Then<T>``` 這個東西

也就是說，每一個功能其實並不需要自己對外宣稱可以如何來串接

我們只需要用擴充方法來加以串接即可

而這件事情勢必會牽涉到串接進出口的模式

隨著模式的不同和串接的不同

所以我們先來看有哪些 Input 與 Output 模式

這裡我提幾個現在API上會比較常見的

 - Input

   - ```GetAsync(HttpRequestMessage request)```

     - 這種屬於已非常明確地公開相依型別的參數 

       - 優點：清楚明確，無須額外轉換可直接使用，且為常用型別
       - 缺點：強制使用者相依於此型別的模組

   - ```GetAsync(Action<HttpRequestMessage> config)```

     - 範例所使用的方式，讓使用者提供設定的方法而不是建立好的物件

       - 優點：物件建立並非由使用者建立，且為常用型別，所以非公開建構子型別的參數帶入很適合
       - 缺點：同上
    
   - ```GetAsync(Action<HttpRequestBuilder> config)```

     - 這作法類似於HtmlElement的概念，讓使用者把資訊建立在 Builder 上，這樣就可以很合理的用 Builder 來建構我們所需要用的物件，而使用者也無須關注是如何達成的

       - 優點：無上述兩種類型的相依性缺陷
       - 缺點：Builder如何使用，設計不良的情況下會無法有足夠好的成效
         - 解決方案：可以嘗試搭配[Specification Pattern](https://en.wikipedia.org/wiki/Specification_pattern) 服用

   - ```GetAsync(IConfigureOptions<HttpRequestMessage> config)```

     - 這是.Net Core Options Model中的型別```IConfigureOptions<>```，其成員只有公開```void Configure(TOption option)```，所以與前述第二和三個一樣，只是這次用了物件加以包裹

       - 優點：更適合OOP的概念，而且可以區分各種Action<>代表之意義
       - 同樣相依於第三方型別問題
         - 解決方案：同樣於上述的做法，只是這次改更換整體的型別名稱，ex:```IHttpRqeuestConfigureOptions```

   - ```GetAsync<TRequest>(TRequest request)```/```GetAsync<TRequest>(Action<TRequest> config)```

     - 透過泛型來提供參數的一種方式，一般來說除非真的能夠泛用在各種型別上時可能會看到這種類型的Input

       - 優點：操作自由度偏高，而且無須額外包裹物件或委派
       - 缺點：操作者既然自由度高就表示程式得承擔相關判斷
         - 解決方案：視情況加個[約束條件](https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters)，或還是改用介面的做法會比較妥當

   - ```GetAsync()```

     - 往往這個意思表示，所需要的設定或物件已經於物件建構時載入，你只需要Invoke

       - 優點：操作容易，簡單易懂，而且沒有多餘的贅述
       - 缺點：如何Get沒有放在此處，如果直接由使用者引用會不知道是在Get什麼東西 ~~所以你還記得你設定了什麼嗎？~~ 
       　 - 解決方案：既然此處已與使用者無關，不如放在裡面讓外部的RunAsync之類的方法來呼叫吧

基本上上述的方式都各有優缺點，而且實際上的確也都可能看的到

下面舉兩個實際案例

### [Moq4](https://github.com/moq/moq4)

```csharp
var lovable = Mock.Of<ILoveThisFramework>(l => l.DownloadExists("2.0.0.0") == true);

Mock.Get(lovable)
    .Verify(framework => framework.DownloadExists("2.0.0.0"));
```

Mock.Of 也是一種介系詞的模式，這個後面會提及

Mock.Get(lovable) 即為泛型的方式，而這裡只是因為編譯器的[型別推斷(Type Inference)](https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/generics/generic-methods)，所以我們不用去詳述其型別

而其後續接Verify驗證並帶入如何驗證

所以這樣的套件作法再加上適合的開發方式，就可以很容易寫出簡單易懂的測試


### [chocolatey](https://github.com/chocolatey/choco)
```csharp
var config = new ConfigureBuilder<ChocolateyConfiguration>()
    .Set(c => c.PackageNames = "docker")
    .Set(c => c.CommandName = "list")
    .Build();

chocolatey.Lets.GetChocolatey()
    .Set(config.Configure)
    .List<IResult>()
    .SelectMany(result => result.Messages,(result,msgs) => msgs.Message)
    .Aggregate(Console.WriteLine,(write,msg) => write(msg));
```
chocolatey 是windows版的apt-get，而理論上操作通常都是用PowerShell 

但仍然可以在NuGet上找到　chocolatey.lib 

所以，上面這code 是我參考 chocolatey.lib 來嘗試的程式

在這次的我的寫法中也仍然是屬於泛型的做法，只是泛型型別再class上

chocolatey 的Set 則是屬於第三種作法，自訂型別的Action<>

而他提供的是委派型別，並不表示我就得寫[Lambda](https://docs.microsoft.com/zh-tw/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions)在裡面

我的ConfigureBuilder在Builder之後的物件，是IConfigureOptions<>

所以透過類似委派型別的方式來達到自然融入套件的效果也是Action<> 這種類型委派的優點

但後續的List<>就很苦手了，IResult我找了好一陣子，而且還不一定對 ~~是的，這段code其實沒有反應~~

所以這也算是一種UX上的缺陷

但.....chocolatey 並沒有推廣他的 C#函式庫，所以某種層面上這樣的寫法可以團隊內部約定

所以就算對外人有UX問題，那等要對外的時候才算數吧


## 備註
---

  - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
  - 選擇第三方套件建議檢查對方是否有測試，除了可以掛保證外，如果不知道要怎麼用也可以從測試得知，畢竟測試即需求
  - chocolatey個人大推，就連visual studio 這種大型軟體都可以自動安裝到好
  - SelectMany(選擇一目標IEnumerable,對前述的目標做Select作為Result)
  - Aggregate(起始與回傳物件, (前一次結果, 這次的物件) => 執行方法內容)
    - 而這裡沒寫到的是我有實作一個擴充方法來輔助這個擴充，就是讓執行方法結束後直接回傳前一次結果，所以才會與常見的做法不一樣，而從頭到尾都是```Console.Write```這個```Action<string>```在來回傳遞