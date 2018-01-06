##  Day-18 這是兩件事
---

由前篇最後做出來的效果，在我看來並不是那麼好

原因如下：

- 依序對應參數帶入的實際上並不是很好的高可讀性作法，因為他無法被描述成子句
- 無法判斷其參數的正確性，或賦予預設值

基於以上兩點，所以我在使用之前的方法，先構想我希望可以如何撰寫

```csharp
    CreateCrawler().GetAsync(ConfigureRequest);

    void ConfigureReqeust(HttpRequestMessage req)
    {
        SetUrl(req);
        SetMethod(req);
        return req;
    }

    void SetUrl(HttpRequestMessage req,string url)
    {
        if(url == null) throw new Exception($"{nameof(url)} is null");
        req.RequestUri = new Uri(url);
    }
    void SetMethod(HttpRequestMessage req,string method)
    {
        method = method ?? HttpMethod.Get;
        req.SetMethod(method);
    }
```

這些只是零稀的片段，不過大約可以做為上述需求的參考

也就是說

- 不要整個設定過程出現在 GetAsync 上 (ex：很大一篇的 Lambda)
- 我也希望每一個Member可以各自獨立定義，這樣才能把每一項設定分開運作
- 分開來運作的目的是可以對輸入的參數做驗證或轉型
- 不能因為上述緣故以至於原本可以設定的部分變成不能設定

~~你PM喔！需求那麼多還那麼討厭~~

不過因為這些項目通通都不屬於Crawler，已經是HttpRequestMessage的事情了

所以，這些內容也通通都不應該出現在Crawler 裡面

而對於HttpRequestMessage的設定，在未來其他地方也很有可能會使用

所以不如好好的設計一番吧

```csharp
    public class HttpRequestMessageBuilder
    {
        private IList<Action<HttpRequestMessage>> _configs;
        public HttpRequestMessageBuilder()
        {
            _configs = new List<Action<HttpRequestMessage>>();
        }

        public void Set(Action<HttpRequestMessage> config)
            => _configs.Add(config);

        public HttpRequestMessage Build(Func<HttpRequestMessage> factory = default)
            => _configs.Aggregate(
                factory() ?? new HttpRequestMessage(),
                (req,config) => config(req));
    }
```

先做個最簡單的設定法

這是透過Set方法把作為設定Request 的方法都先放在集合中

當開始Build的時候，再依序套用在參數帶入的Request Factory 所以建立的 Request 

最後回傳設定好的 Request

至少這已經不太可能會miss掉任何 Request 可以定義的內容

而 Request 可以定義的屬性非常多，實際上我們在開發不會每一個都用到

所以一般就已常用，和當下會用到的來做處理即可

```csharp
    public void SetUrl(Uri url)
        => Set(req => req.RequestUri = url);

    public void SetMethod(HttpMethod method)
        => Set(req => req.Method = method);

    public void SetContent(HttpContent content)
        => Set(req => req.Content = content);

    public void ConfigureHeader(Action<HttpRequestHeaders> config)
        => Set(req => config(req.Headers));
```

這裡用Set 而不是直接_configs.Add 的原因很簡單

除了少幾個字以外，如果我想對於設定做全面的調整時

此時就多了一個 Set 可以用而不需要複寫或擴充 IList<>

這樣就可以符合不使用參數而是使用方法描述來做設定的作法

此時因為常見的幾個方法被分離出來了，所以加上一些驗證和預設值吧

```csharp

    public void SetUrl(Uri url)
        => Set(req => req.RequestUri = (url ?? throw new ArgumentNullException(nameof(url))));

    public void SetMethod(HttpMethod method)
        => Set(req => req.Method = method ?? HttpMethod.Get);

    public void SetContent(HttpContent content)
        => Set(req => req.Content = content ?? new StringContent(string.Empty));

    public void ConfigureHeader(Action<HttpRequestHeaders> config)
        => Set(req => config?.Invoke(req.Headers));

```

這樣不但每一個屬性分離帶入

且驗證也不會交錯複雜，而且也沒有多餘的方法

最後轉型的多載當然是不能影響這些事情

所以接下來就再用擴充漂亮的展開吧


## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - config?.Invoke(arg) 的意思是，如果config不為null則繼續往下做，否則此行直接打斷往下一行開始運作
 - 即使這樣分開設定預設值，何在一起還是有[Template Pattern](http://www.dofactory.com/net/template-method-design-pattern)的概念
 - 每一個Action<>參數都在一次轉變為我們所想要的設定型別，這也屬於[Decorate Pattern](http://www.dofactory.com/net/decorator-design-pattern)
 - Builder class為 [Builder Pattern](http://www.dofactory.com/net/builder-design-pattern)，好處是可以於Builder時期先完成所有我們需要做的驗證，以建構出一個[Factory](http://www.dofactory.com/net/factory-method-design-pattern)，作為快速Create的方法
 - Design Pattern 往往都是先有了基本作法才慢慢去改變或套用模型，並且可以透過Design Pattern 更容易了解對方此時此刻想達成的目的是什麼
 - 你只要忘記所有的招式，就學會Design Pattern了 !!