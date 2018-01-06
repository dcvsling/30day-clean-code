using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ithome.IronMan.Example
{
    public interface ICrawler
    {
        /// <summary>
        /// 依照提供的Request     //HttpRequestMessage
        /// 以非同步方式取得 　　　//GetAsync
        /// Html物件序列          //IEnumerable<HtmlElement>
        /// </summary>
        /// <param name="config">Action[HttpRequestMessage]</param>
        /// <returns>IEnumerable[HtmlElement]</returns>
        Task<IEnumerable<HtmlElement>> GetAsync(HttpRequestMessage request);
    }
}