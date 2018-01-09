## Day-22 來自異鄉的解決方案
---

那麼麻煩的需求，到底要怎麼解決呢？

前一陣子在網路學習平台上看到一個來自於 [Functional Programming](https://en.wikipedia.org/wiki/Functional_programming) 的一個很有趣的模型

這裡就叫他為 Either 吧，寫法大概是這樣

```csharp

public abstract class Either<TLeft,TRight>
{
    public abstract Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapping);

    public abstract Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapping);

    public abstract TLeft Reduce(Func<TRight, TLeft> mapping);
}

```

這個類別的作用類似於 [Strategy Pattern](https://en.wikipedia.org/wiki/Strategy_pattern)

而只是他的決策只有兩個 Left or Right

但這要怎麼使用才會有二選一個效果呢

所以當然不只有這個，現在在把 Left 和 Right 都建立出來吧

```csharp

    public class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        TLeft Value { get; }

        public Left(TLeft value)
        {
            this.Value  = value;
        }

        public override Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapping)
            => new Left<TNewLeft, TRight>(mapping(this.Value));

        public override Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapping)
            => new Left<TLeft, TNewRight>(this.Value);

        public override TLeft Reduce(Func<TRight, TLeft> mapping)
            => this.Value;
    }

    public class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        TRight Value { get; }

        public Right(TRight value)
        {
            this.Value = value;
        }

        public override Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapping)
            => new Right<TNewLeft, TRight>(this.Value);

        public override Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapping)
            => new Right<TLeft, TNewRight>(mapping(this.Value));

        public override TLeft Reduce(Func<TRight, TLeft> mapping)
            => mapping(this.Value);
    }

```

所以實際上是用建構子所帶入的參數來決定實際上被使用的類別

所以實際上的用法大概像是這樣

```csharp
public abstract class MyChoose {
    public Either<LeftType, RightType> Run(object val)
    {
        if(val is RightType right)
            return new Right(right);
        else (val is LeftType left)
            return new Left(left);
        // 此處因為示範緣故，未定義空物件，所以單以名稱表示
        return Left.Empty;
    }
}

```

而最後回傳結果的用法是

```csharp

    string result = either.MapLeft(left => left.ToString())
        .Reduce(right => right.ToString());

```

解釋一下為什麼這樣就可以正確取得我們所要的結果

當我們使用 Left 的時候呼叫 MapLeft 會確實讓我們帶入的參數 Left.ToString() 被執行

並且將結果成為新的 Either<string,RightType> 的建構子參數來建立新的 Either 物件

而如果呼叫 MapRight 時，只做把原本的 LeftType 放進新的 Either<LeftType,TNewRight>

並不會去呼叫參數的方法

所以這時候就會發生無論 MapRight 怎麼呼叫都不會改變實際的結果

反之 Right 也是只處理 RightType 的進展，同時忽略 Left 相關的所有方法

而最後 Reduce 則是將 Right 型別變成 Left 的型別的物件的目的是為了取得最終結果

以上述的範例，如果想要取得字串結果，那我就先把左邊變成字串

最後在 Reduce 把右邊也變成字串

那最後的結果就一定會得到字串

這好像挺接近我們想要的結果

- 可對應於各種回 傳case 的 result
  - 因為泛型的緣故，所以 Left Right 皆可以自行選擇型別，而且可以各自在成為新的 Either
- 同時也要可以提供Exception
  - 只需要讓其中一邊的型別為 Exception 即可
- 必且可以規範使用者如何使用，並使其不會自發或慣性的產生[bad smell](https://en.wikipedia.org/wiki/Code_smell)
  - 我們可以把方法名稱換成合適的名稱，那使用者就自然會知道要如何去呼叫，那使用者就會依循規範取得想要的結果
- ~~我想要通用版本~~
  - 因為為泛行而且可以持續二選一，所以任何需要選擇的狀況應該都適用

補充優點

- 這個模型因為為泛型型別，而且未使用任何的約束條件，所以每一種結果的物件皆可以完全無相依性
- 藉由泛型物件的型別持續於方法中變換，可以讓使用者自行決定最終的結果
- 由於變換型別的方法由使用者自行決定，所以使用者可以使用自己開發的方法來轉換型別
- 轉換方法的執行由提供者來決定是否執行，所以可以要如何去執行最終還是交由提供方

可能適用的方法已經有了，接下來如何將這個解決方案變成我們所期望的結果

就讓我們繼續看下去吧

## 備註：
---

 - 如您目前是團隊工作，則請遵守團隊所訂下的撰寫規範，此篇文章僅為參考建議使用
 - 目前大多數的解決方案，往往都一定會附加其他優勢，這是值得努力的方向
   - Dependency Injection (DI)：作為 [IoC](https://en.wikipedia.org/wiki/Inversion_of_control)的解決方案，並且藉由 [Constructor Injection](https://en.wikipedia.org/wiki/Dependency_injection#Constructor_injection) 來解決複雜的建構以對應 [SOLID](https://en.wikipedia.org/wiki/SOLID) 原則所產生的眾多小類別，而且各加 DI 提供廠商也有開發自己的特色 ex: [Castle](http://www.castleproject.org/) 的 [Dynamic Proxy](http://www.castleproject.org/projects/dynamicproxy/)
   - Unit Test：原本單純的程式碼測試，除了反向操作後所得到新的開發模式 TDD ，並且還可藉由此衍生出 [Kata](https://en.wikipedia.org/wiki/Kata_(programming))，作為測驗與練習的解決方案
   - Open Source：原本最大的困境[盜版與攻擊](https://抱歉.我找不到當初看到的那篇文章了orz....)，藉由[社群](https://en.wikipedia.org/wiki/Community_source)的力量和[商業模式轉型](https://en.wikipedia.org/wiki/As_a_service)，不僅加快了發展速度，而且更產生了容易接近與了解使用者的需求作為發展方向，這樣的良性循環
   - ~~我相信還有很多現代的解決方案都是這樣的，就靠各位看官發現囉~~
 - 本篇所介紹的模型來自於 [Pluralsight](https://app.pluralsight.com/) 中 [Zoran Horvat](https://rs.linkedin.com/in/zoran-horvat) 講師所提供的課程 [Advanced Defensive Programming Techniques](https://app.pluralsight.com/library/courses/advanced-defensive-programming-techniques/table-of-contents) 的最後一節．該節也是將此模型用於 Request Result 和 Exception Handle
 - 個人非常推此位講師的課程，如果對此講師的課程有興趣，可以考慮使用 [Pluralsight 的免費試用](https://billing.pluralsight.com/individual/checkout/account-details?sku=IND-M-PLUS-FT)，或是註冊微軟的[Visual Studio Dev Essentials](https://www.visualstudio.com/zh-tw/dev-essentials/)，裡面的 Benefit 其中一項就是 Pluralsight 的 3個月免費訂閱
 - ~~如果你已經用完了，就辦個新帳號再來一次吧~~