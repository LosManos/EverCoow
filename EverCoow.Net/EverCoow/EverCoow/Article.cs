using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EverCoow
{
    public class Article
    {
        public string Title { get; set; }
        public Content CdataContent { get; set; }

        internal static Article Create(XmlNode element)
        {
            var title = element.SelectSingleNode("title").InnerText;
            var cdataContent = element.SelectSingleNode("content").InnerXml;
            return Create(title, Content.Create(cdataContent));
        }

        internal static Article Create(string title, Content cdata)
        {
            return new Article().Set(title, cdata);
        }

        private Article Set(string title, Content cdata)
        {
            this.Title = title;
            this.CdataContent = cdata;
            return this;
        }
    }
}
