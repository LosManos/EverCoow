using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace EverCoow
{
    /// <summary>This class is for handling Enex-Markdown to email-string conversion.
    /// Example of imput:
    /// <en-note style="word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;">
    ///     Eating cows is like
    ///     <div>eating a port of your</div>
    ///     <div>soul.</div>
    ///     <div><br/></div>
    ///     <div>- http://example.com/     &lt;- with comment</div>
    ///     </en-note>
    /// </summary>
    public class MarkDownConverter : IMarkDownConverter, IMarkDownUnitTest
    {
        /// <summary>This method converts an enex xml doc formatted approx as in the xml comment
        /// to a Markdown.  A simplified Markdown.
        /// </summary>
        /// <param name="enexDoc"></param>
        /// <returns></returns>
        public string Convert(System.Xml.XmlDocument enexDoc)
        {
            var resList = new List<string>();
            ConvertDivTagsToStringlist(enexDoc.SelectSingleNode(Constants.ElementName.EnexNote), resList);
            ConvertBrToCrlf(resList);
            ConvertLink(resList);
            return string.Join(Constants.ElementName.BrTag, resList);
        }

        /// <summary>This method converts divs that consist of only a <br/> to nothing.
        /// </summary>
        /// <param name="rowList"></param>
        private static void ConvertBrToCrlf(IList<string> rowList)
        {
            for (var index = 0; index < rowList.Count; ++index)
            {
                var row = rowList[index];
                if (row.ToLower() == "<" + Constants.ElementName.Br.ToLower() + @" />") //  TODO:This looks like crap.
                {
                    row = string.Empty;
                }
                rowList[index] = row;
            }
        }

        /// <summary>This method converts div tags to text and leaves text as text.
        /// </summary>
        /// <param name="xmlElement"></param>
        /// <param name="resList"></param>
        private void ConvertDivTagsToStringlist(XmlNode xmlElement, List<string> resList)
        {
            foreach (XmlNode elem in xmlElement)
            {
                switch (elem.Name)
                {
                    case "#text":
                        resList.Add(elem.InnerText.Trim());
                        continue;
                    case Constants.ElementName.Div:
                        resList.Add(elem.InnerXml.Trim());
                        continue;
                    default:
                        throw new NotImplementedException("There is presently only code to handle DIV tags.");
                }
            }
        }

        /// <summary>This method converts link strings to Markdown links.
        /// </summary>
        /// <param name="rowList"></param>
        private void ConvertLink(List<string> rowList)
        {
            for (var index = 0; index < rowList.Count; ++index)
            {
                var row = rowList[index];
                rowList[index] =
                    row.TrimStart().StartsWith("- http") ?
                    ConvertLink(row) :
                    row;
            }
        }

        /// <summary>This method converts a string row containing a link to a link.
        /// - http://example.co1/
        /// - https://example.co2/    
        /// - http://example.co3/     &lt;- with comment
        /// - https://example.co4/     &lt;- with comment
        /// 
        /// ^-\s(https?:\/\/\S+)(\s*|(.*x-(.*)))?$
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string ConvertLink(string row)
        {
            Debug.Assert(row.StartsWith("- http"));

            var matches = Regex.Matches( row, @"^-\s(https?:\/\/\S+)(\s*|(.*&lt;-(.*)))?$");
            var url = matches[0].Groups[1].Value.Trim();
            var comment = matches[0].Groups[4].Value.Trim();
            //var commentWithPrefix =
            //    string.IsNullOrWhiteSpace(comment) ?
            //    string.Empty :
            //    "\t<- " + comment;

            ////  [This link](http://example.net/) <-and a comment
            //return string.Format("[{0}]({0}){1}", url, commentWithPrefix);

            var commentWithPrefix = 
                string.IsNullOrWhiteSpace(comment )?
                string.Empty :
                "&nbsp;&nbsp;&lt;- " + comment;
            return string.Format( "<a href=\"{0}\">{0}</a>{1}", url, commentWithPrefix );
        }

        /// <summary>This is an interface method just to get unit tests working in an easy way.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        string IMarkDownUnitTest.ConvertLink(string row)
        {
            return ConvertLink(row);
        }

    }
}
