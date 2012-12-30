using EverCoow.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Pair = System.Collections.Generic.KeyValuePair<string, string>;

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
    public class EverCoowMarkDownConverter : IEverCoowMarkDownConverter, IMarkDownUnitTest
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
            ConvertMDECLink(resList);
            RemoveTrailingBRs(resList);
            return string.Join(Constants.ElementName.BrTag + Environment.NewLine, resList).Trim();  //  Trim also removes newline et al.
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
            var row = string.Empty;

            foreach (XmlNode elem in xmlElement)
            {
                switch (elem.Name)
                {
                    case "#text":
                        //resList.Add(elem.InnerText.Trim());
                        row += elem.OuterXml;
                        continue;
                    case Constants.ElementName.A:
                        var atag = CreateEmailUrlFromAtag(elem.OuterXml);
                        //resList.Add(atag);
                        row += atag;
                        continue;
                    case Constants.ElementName.Br:
                        //resList.Add(string.Empty);
                        //resList.Add(ret);
                        //ret = string.Empty;
                        continue;
                    case Constants.ElementName.Div:
                        if (string.Empty != row)
                        {
                            resList.Add(row);
                            row = string.Empty;
                        }
                        var onlyText = elem.InnerText.Trim() == elem.InnerXml.Trim();
                        if (onlyText)
                        {
                            //resList.Add(elem.InnerText.Trim());
                            resList.Add(row + elem.InnerText);
                            row = string.Empty;
                        }
                        else
                        {
                            //  We have XML inside too.
                            if (string.Empty != row)
                            {
                                resList.Add(row);
                                row = string.Empty;
                            }
                            var doc = new XmlDocument();
                            doc.LoadXml("<X>" + elem.InnerXml + "</X>");
                            ConvertDivTagsToStringlist(doc.SelectSingleNode("X"), resList);
                            //resList.Add(elem.InnerXml.Trim());
                        }
                        continue;
                    case "X":
                        continue;
                    default:
                        throw new NotImplementedException("There is presently only code to handle DIV tags, received element name was [" + elem.Name + "].");
                }
            }

            resList.Add(row);
        }

        /// <summary>This method converts link strings to Markdown links.
        /// </summary>
        /// <param name="rowList"></param>
        private void ConvertMDECLink(List<string> rowList)
        {
            for (var index = 0; index < rowList.Count; ++index)
            {
                var row = rowList[index];
                if (IsXml(row))
                {
                    //  Leave be.
                }
                else
                {
                    rowList[index] =
                        row.TrimStart().StartsWith("- http") || row.Contains("http:") ?
                        ConvertMDECLink(row) :
                        row;
                }
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
        private string ConvertMDECLink(string row)
        {
            //Debug.Assert(row.StartsWith("- http") || row.Contains("http"));

            bool changed = MatchAndConvertExplicitMDECLink(ref row) ||
                MatchAndConvertImplicitMDECLink(ref row);

            return row;
        }

        private string CreateEmailUrl(string url)
        {
            return string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", url);
        }

        /// <summary>The parameters should be as an a-tag.
        /// [a href="example.com"]mylink[/a]
        /// </summary>
        /// <param name="aTag"></param>
        /// <returns></returns>
        private string CreateEmailUrlFromAtag(string aTag)
        {
            var doc = new XmlDocument();
            doc.LoadXml(aTag);  
            var targetAttribute = doc.CreateAttribute("target");
            targetAttribute.Value = "_blank";
            doc.DocumentElement.Attributes.Append(
                targetAttribute
            );

            var ret = doc.InnerXml;

            //  Fix for bug in Evernote:
            //  http://discussion.evernote.com/topic/32756-export-bug-ampersands-in-url-gets-html-formatted-mac-v-500-400524/
            //  Strange: when I do doc.DocumentElement.Attributes["href"].value I get
            //  the & correctly.  But as doc.InnerXml I get &amp;
            ret = ret.Replace("&amp;", "&");  //  Does not work when the ampersand is in the text of the a tag.  It does replace then.  But we should keep it HTML formatted.

            return ret;
        }

        /// <summary>This is an interface method just to get unit tests working in an easy way.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        string IMarkDownUnitTest.ConvertMDECLink(string row)
        {
            return ConvertMDECLink(row);
        }

        private const string ExplicitMDECLinkPattern = @"^-\s(https?:\/\/\S+)(\s*|(.*&lt;-(.*)))?$";

        //  Test patterns.
        //  http://example.com
        //  http://example.com/
        //  A http://example.com/
        //  http://example.com/ B
        //  A http://example.com/ B
        //  A http://example.com/ B https://xx C
        //  http://example.com/id
        //  http://www.example.com/show.aspx?id=12
        //  A http://www.example.com/show.aspx?id=12 B
        private const string ImplicitMDECLinkPattern = @"https?:\/\/[\S]+.*?";

        /// <summary>This is a very simple check if a string is Xml.
        /// A "better" solution is found here:
        /// http://stackoverflow.com/questions/1490053/how-to-tell-if-a-string-is-xml
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsXml(string row)
        {
            row = row.Trim();
            return row.StartsWith("<") && row.EndsWith(">");
        }

        private bool MatchAndConvertExplicitMDECLink(ref string row)
        {
            var matches = Regex.Matches(row, ExplicitMDECLinkPattern);
            switch (matches.Count)
            {
                case 0:
                    //  Leave row as is.
                    return false;   //  No hit.
                case 1:
                    var url = matches[0].Groups[1].Value.Trim();
                    var comment = matches[0].Groups[4].Value.Trim();
                    var commentWithPrefix =
                        string.IsNullOrWhiteSpace(comment) ?
                        string.Empty :
                        "\t\t&lt;- " + comment;
                    row = string.Format("- <a href=\"{0}\" target=\"_blank\">{0}</a>{1}", url, commentWithPrefix);
                    return true;    //  Hit and change.
                default:
                    throw new Exception(string.Format("The row [{0}] seems to contain too many URLs."));
            }
        }

        /// <summary>This method searches the parameter for somethid that looks like 
        /// my custom markdown (what i call evernotemarkdown)
        /// - http://www.example.com/   [- and a comment.
        /// and converts to something to be used in an email like
        /// [a href="http..."]http...[/a]  [- and a comment.
        /// </summary>
        /// <param name="row">This parameter is both input and output.</param>
        /// <returns>True if the string was manipulated.</returns>
        private bool MatchAndConvertImplicitMDECLink(ref string row)
        {
            var matches = Regex.Matches(row, ImplicitMDECLinkPattern);
            if (matches.Count >= 1)
            {
                var urlList = new List<string>();
                foreach (Match match in matches)
                {
                    //  URLs inside an A tag happens to end with " due to how the regex is written.
                    if (false == match.Value.EndsWith("\""))
                    {
                        var url = match.Value;
                        urlList.Add(url);
                    }
                }

                //  TODO: Fix that if we first replace ABCD with <ABCD> and then replace
                //  ABC we will get <<ABC>D>.
                urlList = urlList.Distinct().ToList();
                foreach (string url in urlList)
                {
                    row = row.Replace(url, CreateEmailUrl(url));
                }

                return true;
            }
            else
            {
                return false;  //   No hit.  Leave row as is.
            }
        }

        private void RemoveTrailingBRs(List<string> resList)
        {
            var last = resList.Last();
            if (null != last && last == string.Empty)
            {
                resList.RemoveAt(resList.Count() - 1);
                RemoveTrailingBRs(resList);
            }
        }

    }
}
