## Day-2 又是爬蟲!?
---
最近因為剛好幫朋友寫個爬蟲，在撰寫的過程中發現這是個好素材，所以就拿來用了．

所以我們就來寫個爬蟲吧

以下的會以 .Net Core 2.0 ~ 2.1 (就是還沒release的那一版) 為範例環境

先做一次概略描述

爬蟲感覺功能需求如下：
 - 具有HttpReqeuest, HttpResponse 這樣的站台資訊發送與接收
 - 可以解析抓回來的資訊
 - 解析完有後續還有一個目的 (總不是爬完吃飽沒事幹吧)

感覺好像很複雜，換一個方式來呈現我們的需求吧

 - HttpClient
 - HtmlAgilityPack
 - 先隨便寫個目的吧

所以我們就可以很快速度生成一段code 如下並附加一些解讀　(此註解寫法非正規，僅為表現效果而做)：
```
public class Crawler                                          // 這是爬蟲
{
    public List<HtmlNode> Start(string url)                   // 依照參數網址開始運行　並回傳結果
    {
        var client = new HttpClient();                        // 建立Httpclient物件
        var res = client.GetAsync(url).Result;                // 用client以非同步 GET 的方式去指定網址取得回應並等待結果
        var stream = res.Content.ReadAsStreamAsync().Result;  // 將取回的Content再以非同步的方式讀取內容串流並等待結果
        var docs = new HtmlDocument();                        // 建立HtmlDocument物件
        docs.Load(stream);                                    // 讀取剛剛取出的串流
        return docs.DocumentNode.Elements("a").ToList();      // 回傳HtmlDocumentNode中所有Element名稱為"a"的Element
                                                              // 並且放入List中回傳
    }
}
```

這樣的寫法所對應出來的描述，大概就像是這樣

而這樣的描述與這樣的程式碼也許看起來沒有什麼問題

那接下來就來了解一下這次的主題會如何檢驗這段程式吧

備註
---

 - .net core 相關sdk 與　runtime 可至下方官方來源取得
   - [微軟官網](https://www.microsoft.com/net/download/windows)
   - 覺得官網沒有工程師的feel 可以至[Github](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.1.2-sdk-download.md)
   - 你說工程師就是要用[Script](https://github.com/dotnet/docs/blob/master/docs/core/tools/dotnet-install-script.md)來全自動安裝？
 - 此範例程式內容主軸並非完整達成crawler 所以任何細節請參考以crawler為主題的內容為主
