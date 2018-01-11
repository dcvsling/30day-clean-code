## Day-24 是通用，也是專用Z
---

一般來說，軟體套件的廠商基於套件適用對象的緣故

所以會使用開發人員所能理解的描述

例如：Controller , Provider , Service

而我們在使用的時候，其實可以以更適用於人常用的名稱

例如：Store , Ticket , Find

所以同樣的，我們也不應該直接使用 ```Either``` 更何況是　```Either<TLeft,TRight>```

這裡所需要實作的功能是 Result 而產生 Result 的是 crawler

所以才會命名為 ```CrawlerResult```

而泛行使用的考量為，Result 提供方一定總是提供```IHtmlElementCollection```

而例外雖然可能會有不同，但是全部都是```Exception```的衍生

所以先定義出```CrawlerResult<TResult,Exception>```

們在定義```CrawlerResult : CrawlerResult<IHtmlElementCollection,Exception>```

並且在各自定義```OkResult<TResult,Exception>``` ```ErrorResult<TResult,Exception>```

然後都繼承```CrawlerResult<TResult,Exception>```

這盎就跟原本的解決方案相似了

再來就是所使用的方法

我們保留 Left 方法，但是我們並不會需要讓例外錯誤的物件一直往下延展

反而是我們應該要持續讓他可以 Handle 不同的例外

所以將參數從原本的 ```Func<T,TNew>``` 變成 ```Func<TException,Exception> where TException : Exception```

這樣就可以讓使用者自己決定要 Handle 哪個例外，甚至是不想要讓例外被擲出時

所以裡面的程式也需要稍作調整，先行判斷是否為該例外型別，不是就跳過

而最後如果仍然拿到 Exception 物件，就擲出例外

因此我們就不會再需要泛型的 Right 的部分

所以最後的模型為 ```CrawlerResult<T>``` 並且起始回傳的型別為 ```CrawlerResult```

而泛型的轉換則會在方法呼叫的過程中，由編譯器自動識別

並且最終得到使用者所想要的結果

而基於 Result 提供方的開發方便性

我們建立一個```CrawlerResult```抽象類別，並且定義Ok與Error方法

而 Ok 就是接收```IHtmlElementCollection``` ，Error 則是接收任何一種```Exception```

並且分別建立```OkResult```與```ErrorResult```，並且都轉為CralwerResult回傳

如此一來這個通用的解決方案，就變成像是用於當下功能的專用解決方案

基於上述的說明，所以接下來可以在繼續做一些調整

## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 泛型並不會造成開發上看起來太過複雜的情況，只要於封裝前後都有收尾，一樣也可以很好用
   - 例如：[Autofac](https://github.com/autofac/Autofac/blob/develop/src/Autofac/Builder/IRegistrationBuilder.cs)
   - 或是更精采的：[Microsoft.AspNetCore.Identity](https://github.com/aspnet/Identity/blob/dev/src/Service.EF/IdentityServiceDbContext.cs#L42)
