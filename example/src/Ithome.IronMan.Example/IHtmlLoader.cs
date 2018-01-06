using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;

namespace Ithome.IronMan.Example
{
    /// <summary>
    /// 讀取Stream中的Html
    /// </summary>
    public interface IHtmlLoader
    {
        /// <summary>
        /// 從Stream中讀取Html節點
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>html節點</returns>
        IEnumerable<HtmlNode> Load(Stream stream);
    }
}