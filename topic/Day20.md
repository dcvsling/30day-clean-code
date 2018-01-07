# Day-20 在檯面上覆蓋一份測試 結束這回合
--

最後加上一組簡單的[測試](https://github.com/dcvsling/30day-clean-code/blob/Day20/example/test/Ithome.IronMan.Example.Extensions.Tests/HttpRequestMessageBuilderTests.cs)

上半部如何擴展及銜接，並且在符合原則下提供自由度與可讀性

大概就介紹到此

再往下之前，先提一件事情

前面講了那麼長一篇的做法，其實可能已經不需要這麼麻煩了

因為 [DI](https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/dependency-injection#what-is-dependency-injection) 就可以幫我們很輕易地完成這件事情

這裡簡單介紹一下，.Net Team 的 [DI Container](https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/dependency-injection#replacing-the-default-services-container)

[Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection)

這個元件是 Base On NetStandard 2.0 所以，無論是否為 .Net Core 都可以使用

用法也很簡單，這裡就不多加贅述，繼續講跟目前有關的事情吧

依照範例的狀況，來做個簡單的用例

```csharp

var services = new ServiceCollection();
services.AddOptions()
    .Configure<HttpRequestMessage>(ConfigureRequest);
var provider = services.BuildServiceProvider();
var request = provider.GetService<IOptions<HttpRequestMessage>>().Value;

void ConfigureRequest(HttpRequestMessage request)
{
    request.RequestUri = new Uri("https://127.0.0.1/");
    request.Method = HttpMethod.Get;
}
```

這樣就就可以做完前面所做的事情

當然我們可以繼續接著寫

```csharp
var services = new ServiceCollection();
var provider = services.AddOptions()
    .Configure<HttpRequestMessage>(ConfigureRequest)
    .AddTransient<ICrawler,Crawler>()
    .AddTransient<IHtmlLoader,DefaultHtmlLoader>()
    .AddTransient<IHtmlLoader,DefaultHttpClient>()
    .BuildServiceProvider();
var htmls = provder.GetService<ICrawler>().GetAsync(provider.GetService<HttpRequestMessage>());
```

要在誇張一點都可以

```csharp
var services = new ServiceCollection();
var provider = services.AddOptions()
    .Configure<HttpRequestMessage>(ConfigureRequest)
    .AddTransient<ICrawler,Crawler>()
    .AddTransient<IHtmlLoader,DefaultHtmlLoader>()
    .AddTransient<IHtmlLoader,DefaultHttpClient>()
    .AddTransient<IHtmlElementCollection>(
        p => provder.GetService<ICrawler>().GetAsync(provider.GetService<HttpRequestMessage>()).Result)
    .BuildServiceProvider();
var htmls = provder.GetService<IHtmlElementCollection>();
```

有沒有一種很Docker的感覺呢? 

而且這樣子描述明確，清楚的呈現所使用的類別

所以 DI 可以用很清楚的方式幫我們自動建構起複雜建構的服務

而且可以取得任何於容器中註冊的物件 ~~越來越Docker~~

如果覺得這樣還是太過於複雜混亂

仍然可以用之前的方法來重新用方法裝載，所以這種做法可不是空穴來風

用一個曾經有機會上戰場的 Startup 來當作結尾吧

```csharp

public class Startup 
{
    public Startup(IHostingEnvironment env)
    {
        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .SetContentPath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public IConfiguration Configuration { get; }

    public void ConfigureService(IServiceCollection services)
        => services.AddOptions()
            .AddSingleton(Configuration)
            .AddLogging()
            .AddAntiforgery()
            .AddAuthentication()
                .AddCookie().AddGoogle().AddFacebook().Services
            .AddIdentity()
            .AddElm()
            .AddSwaggerGen(o => o.SwaggerDoc("v1", new Info { Title = "My API V1", Version = "v1" }));
            .AddBlogs()
            .AddEntityFrameworkCoreSqlServer()
            .AddFileServer()
            .ConfigureAll<RewriteOptions>(o => o.AddRedirectToHttpsPermanent())
            .AddDbContext<BlogDbContext>(
                (p,builder) => builder.UseSqlServer(
                    p.GetService<IConfiguration>().GetConnectionString("DefaultConnectString")))
            .AddMvcCore()
                .AddControllersAsServices()
                .AddApiExplorer();
                
    public void Configure(IApplicationBuilder app)
    {
        var factory = app.ApplicationServices.GetService<ILoggerFactory>().AddConsole().AddDebug();

        var env = app.ApplicationServices.GetService<IHostingEnvironment>();

        app.UseElmCapture();

        if(env.IsDevelopment())
        {
            app.UseElmPage()
                .UseDeveloperExceptionPage()
                .UseDatabaseError()
        }    
        else
        {
            app.UseExceptionHandler();
        }
        app.UseRewriter()
            .UseAuthentication()
            .UseFileServer()
            .UseSwagger()
            .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"))
            .UseMvc()
            .UseStatusCodePage();

    }
}

```

現在已經又可以在簡化了，這裏特別拿出來的目的就是

許許多多的服務，用一個類別一次條列並完成註冊

雖然看似複雜，但是每一個項目也都還是清晰可見

而且也同時擁有非常高的自由度和擴展性

於此同時，這也是非常明確的物件導向封裝

## 備註：
---

 - 本次範例程式碼以更新至[Github](https://github.com/dcvsling/30day-clean-code/tree/Day20)
 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - DependencyInjection 搭配 Options 是絕配　XD
 - 上方描述的方法名稱等同於該服務內容，有興趣的可以挑幾個來研究研究
 - 以上省略了非常非常非常多的內容未解釋，請參閱[新版文件](https://docs.microsoft.com/zh-tw/aspnet/core/)