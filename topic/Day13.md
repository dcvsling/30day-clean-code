## Day-13 命名也講三字經!?
---

在說完方法之後，來說說類別的命名方式

這裡借用一下 DDD 的一個詞彙叫做 Domain 中文可以稱為定義域

所謂的定義域是指包含主要描述對象的所有相關元件的範圍

以先前寫的範例來說，爬蟲本身就是一個可以做為主要描述對象的東西

接下來就以爬蟲作為範本來說明如何命名吧

 - {main}：為 Domain 中主要的功能

   - Ok：Crawler

   - NoOk：HttpGetter

     - 盡可能用一個英文單字去描述這個主要功能，也盡量不要使用行動者作為名稱　ex:Getter




 - {main}{Actor}：為 Domain 中與主要功能有關的行動者

   - Ok : CrawlerBuilder, CrawlerRunner

   - NoOk：BuildCrawler, CrawlerGo

   - Crawler建立者會建立Crawler ex: Crawler Build() { }

     - 但建立Crawler已經是個動作，如果在呼叫 Build 那就重複了
     - 而行動也不是行動者，也會產生上述的問題
~~沒有天生技可以動兩次這種事情~~




 - {decorate}{main}：為主要功能的追加描述 ~~亞種~~

   - Ok：HttpCrawler, ApiCrawler

   - NoOk：CrawlerForHttp, JsonCrawler

     - "For" 這個介詞會在別的地方有作用，
如果真的必須用到這種介詞，有可能是作用功能太多

     - 而如果改爬Api的Json ~~如果你很堅持想用Crawler~~

     - 也應該是爬Api而不是爬Json，Json是由別的功能來處理的




 - {decorate}{main}{actor} : 上述案例全部組合再一起也行

   - Ok：HttpCrawlerBuilder, ApiCrawlerRunnter

   - NoOk：BuildCrawlerForHttp, JsonCrawlerGo

     - 這裡會再次突顯NoOK的理由，這樣的描述已經不是一個行動者，反而更像是個標語

   - NoOk：BuildHttpGetterForHttp, JsonJsonGetterGo,

     - 而這裡再往前一個NoOK套用就會出現重複



講了那麼多其實就像是單元測試的 3A 寫法

類別命名最多也是三個字節，每一個字節各代表著一個描述

而這樣的描述會完全都在表達同一件事情

也就是 Domain 所定義的事情，在先前範例終究是 Crawler

透過類別名稱，我們可以不用進入詳細程式碼就得知

Crawler 是如何被產生，具有哪幾種類型，甚至需要什麼運行方式

~~這次請到.Net Team 來為我們示範一下~~

![https://github.com/aspnet/Logging/tree/dev/src/Microsoft.Extensions.Logging](https://github.com/dcvsling/30day-clean-code/blob/Day13/resources/Day13.png "from Microsoft.Extensions.Logging")

這裡我做一個大概的整理

 - Logging
   - {Logging}{Builder}
 - Builder 
   - {FilterLogging}{Builder}{Extensions}
   - {ILogging}{Builder}
   - {Logging}{Builder}{Extensions}
 - Logger
   - {Logger}
   - {Logger}{Factory}
   - {Logger}{Information}
 - Filter
   - {Logger}{Filter}{Options}
   - {Logger}{Filter}{Rule}
 - Options
   - {StaticFilter}{Options}{Monitor}
 - Rule
   - {Logger}{Rule}{Selector}
 - LoggerLevel
   - {Default}{LoggerLevel}{ConfigureOptions}
 - ProviderAlias
   - {ProviderAlias}{Attribute}
   - {ProviderAlias}{Utilities}
 - ServiceCollection 
   - {Logging}{ServiceCollection}{Extensions}
 
而這大概就是上述所描述的情況，即便後續存在著分支，也通通是圍繞著Logging這件事情

Logger 就是很明顯的主要功能

Logger 的 Builder 與 Logging 的 Builder 各自建構了一部分的整體

Extensions 表示擴充方法所在位置，亦表示了前述方法的擴充

Filter 為輔助 Logger 的功能

Options 為 Filter的選項性設定

Rule 規範 Filter 的使用情況所以存在 Selector 選擇器

Logger 存在 LoggerLevel 來決定等級

DefaultLoggerLevel 就是預設的 LoggerLevel

Provider 就是指 LoggerProvider 

ProviderAlias 就是 Provider別名

Attribute 字尾通常是指 C# Attribute 

Utilities 表示目標項目的相關公開方法的總集

SelectCollection　與　ConfigureOptions 這兩個比較特殊

SelectCollection 是 DI 的類別，所以表示有支援DI功能

ConfigureOptions 是 Options 的類別，表示可透過Options來Configure

這樣至少為看先猜可以猜個八九不離十，無論是撰寫還是閱讀都適用

## 備註
---

　- 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
　- 在設計的過程中，不妨也可以嘗試看看以這種方式來描述自己所設計的類別
　- 方法不只一種，像是JS的寫法也沒Follow大小寫
　- 命名的方法當然也適用Design Pattern ~~有些看起來像是沒有follow慣例的請自行帶入Composition Pattern~~