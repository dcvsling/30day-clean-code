# Day-8 此路不通，那就另闢戰場吧
---

**`此篇為C#專屬功能介紹，其他語言可能有該語言專屬相關解決方案`**

**`我將前篇範例中的 ChainAsync/IChainAsnyc 改名為 ChainAwait/IChainAwait`**

如果像是遇到前篇最後那種不符合語法規範，或是一些提供的使用方法不足夠

甚至是像本主題一樣，想把所有羅即全部串起來

這時候在C#中 [擴充方法](https://docs.microsoft.com/zh-tw/dotnet/csharp/methods#extension-methods) 就非常的重要

已下面即將鑰使用的方法作為介紹範例

```csharp
namespace Ithome.IronMan.Example.Fluent
{
    public static class ChainHelper
    {
        /// <summary>
        /// 接著走下一步到下一個非同步階段
        /// </summary>
        /// <param name="next">下一步</param>
        /// <returns>分同步階段</returns>
        public static IChainAwaiter<TNext> Then<T,TNext>(this IChain<T> chain,Func<T, Task<TNext>> next)
            => new ChainAwaiter<TNext>(next(chain.Result));
    }
}

```
擴充方法識別的條件如下
 - 方法所在型別為非巢狀的靜態型別中
 - 方法為上述型別中的靜態方法
 - 方法的第一個參數加上```this```已表示被擴充型別

擴充方法使用條件如下
 - 必須參考方法所存在的組件
 - 使用時必須Using 該擴充方法所存在的namespace

作法慣例
 - 擴充方法及其型別的namespace 與該擴充型別相同
 - 或是統一將其放在某一分類的所指定的namespace下

擴充方法具有對於任意非靜態型別可呼叫之方法的合法擴充

所謂合法是指符合其語言規範，像是如果擴充時該型別某些成員看不到的仍然無法使用 ex: internal

而擴充方法從 .Net 3.0 開始就存在的功能 (仍為2.0 得仰賴其他解決方案)

最容易看到的地方應該會是 System.Linq

而該功能只是將一個第一個參數所指定的型別的物件作為第一個參數

再加上其他參數一起帶入執行這個靜態方法而已

這樣的描述就可以解決我們遭遇的困境

因為擴充方法所在其型別與我們原本的方法並不相同的緣故

所以我們在使用時可能會使用擴充方法，也可能會使用原生方法

並依照所使用的方法的不同也會回傳不同的回傳型別

在方法使用的那一方使用時就可能會存在同名方法具有不同回傳值的情境

因此我們就可以使用 Then 方法來回傳 IChain<> 或是 IChainAwaiter<>

但只是這樣仍然是不足夠的

因為在同名方法上系統仍然會使用原生的方法而不是擴充的

所以接下來會在介紹，如何去使用泛型已達到我們所想要的情境

### 擴充方法用例與注意事項
---
基於前些日子聽聞朋友說，沒看過有人這樣用擴充方法的 (我是擴充方法重度使用者)

於此想對於擴充方法再多追述一些資訊

擴充方法存在的目的是為了

讓單一型別不再因為其可泛用性過多而導致每次加入新方法都得重新編譯或繼承原生型別

並且其擴充之方法仍可與指定的型別共用其規範

在C#上很多Api之所以那麼易懂好用，大多也都歸功於此

這裡舉一個我最近一次看到認為這樣寫很有趣的擴充方法使用案例給大家

在 .Net Core 中，官方的元件存在著這樣的一個慣例
 - Microsoft.Extensions.DependencyInjection　為DI 也就是　IServiceCollection　的擴充型別namepsace
 - Microsoft.AspNetCore.Builder 為　IApplicationBuilder 的擴充型別　namespace
 - 上述namespace 並不為一個　package 而是由各個元件一起補齊的 (所以才叫組件阿~)
 - 所有與 IServiceCollection 有關的擴充方法都放在　ServiceCollectionExtensions 類別中 (幾乎)
 - 所有與 IApplicationBuilder 有關的擴充方法都放在　(Application)BuilderExtensions 類別中 (幾乎)

這樣的作法造成了一個在.Net Core 開發上一個很有趣而且很簡單的情境

後續我還是會回頭來介紹這件事情，先賣個關子

[這裡附上](https://dcvsling.gitbooks.io/tech-descript/content/CSharp/ExtensionMethod/Timing.html) 先前為了提供給朋友而整理出來我可能會寫成擴充方法的判斷方式

也提供給大家作為使用時機的參考 (雖然我覺得你覺得需要就用　沒有合不合適的時機問題就是了)
