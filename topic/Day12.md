# Day-12 這方法叫做大中天 (以下省略)
---

先來說說方法命名吧

一般來說方法多會以動作來命名

ex：Get, Set, InvokeAsync, FindAll, Query

而像是我們前面所使用的範例也有

ex：Load, GetContentAsync, SendAsync

名稱的格式通常如下：

```

{Behavior)：僅描述動作的狀況通常表示，此行為與所屬型別有關

Ok ： ```void Repository<TProduct>.Add(TProduct product)``` repository of products Add a item

NoOK : ```void Repository<TProduct>.AddProduct(TProduct product)``` repository of products add product a item

這裡的問題是，文字上很明顯多了Product，而且Repository<TProduct>　也不應該加入非Product的 item

```

```
{behavior}{target}: 這種需要額外追述的表示對象不像上述那樣通常為自己存在的型別，所以額外描述對象

Ok：```Task<Stream> HttpResponseMessage.GetContentAsync()``` response get content

NoOk：```Task<Stream> HttpResponseMessage.Get()``` response get

這裡的問題則是，Get的對象即便知道是Stream ，也不知道是哪來的Stream，而HttpResponseMessage 本身並不是Stream
```

```
{behavior}{target}{effect) / {behavior}{effect)：除了上述的描述外，追加此動作的影響

OK：```Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)```

NoOK：```Task<HttpResponseMessage> Send(HttpRequestMessage request)```

Async 是動作所造成的副效果，所以導致回傳並非原本所預期的 HttpResponseMessage 所以最後追述
```

```
{behavior}{target}{relation}：表示此行為對象是target的關係者

OK：```FindHistoryBilling(User user)``` find history billing of user

NoOK：```FindUsersHistoryBillingWithAccountAndUsersMon(User user)``` 太長了....

通常超過一個行為就表示該寫第二個方法來接續，不但符合原則也比較清楚

如果真的想這樣寫，那不如直接 FindMon(User user) find mon of user 也許更淺顯易懂
```

這裡再額外提幾個狀況

 - 別因為都是取資料就總是只有 Get ，雖然也不見得有問題但是相較之下就較為生硬
 - ```IServiceProvider.GetService(Type type)```這種不是只有Get 的理由是Service 範圍太廣，而且被擴充的機會很高，所以才會至少呼應Provider來GetService
 - Repository可能比Insert/Delete更適合用Add/Remove
 - 即便上語意可能很接近，但是盡量避免 Add/Delegate，Begin/Stop 這種類型的用法
 - 如果不知道標準在哪，請找旁邊的人幫你念一次然後看他眉頭皺的有多深，就表示問題有多大 XD


## 備註
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 如果您想這樣用但是感覺有卡住，歡迎討論
 - 此內容是在下看了不少.Net Core 的 code 和近期技術所想到的方法，所以如有雷同純屬巧合
 - 基於方法不會只有一種的緣故，所以目的只要寫到讓人看得懂即可，這是核心目的
