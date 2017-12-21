using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Ithome.IronMan.Example
{
    public interface ICrawler
    {
        /// <summary>
        /// 依照提供的Request定義 //Action<HttpRequestMessage>
        /// 以非同步方式取得 　　　//GetAsync
        /// Html物件序列          //IEnumerable<HtmlElement>
        /// </summary>
        /// <param name="config">Action[HttpRequestMessage]</param>
        /// <returns>IEnumerable[HtmlElement]</returns>
        IEnumerable<HtmlElement> GetAsync(Action<HttpRequestMessage> config);
    }
}