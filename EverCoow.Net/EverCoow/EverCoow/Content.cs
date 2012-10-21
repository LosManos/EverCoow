using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EverCoow
{
    public class Content :Cdata
    {
        public string MailText
        {
            get
            {
                //var ret = new List<string>();
                //foreach (XmlNode node in InnerXml.SelectNodes("/en-note/div"))
                //{
                //    ret.Add(node.InnerXml);
                //}
                //return string.Empty;
                return InnerXml.InnerXml;
            }
        }

        internal static Content Create(string cdataText)
        {
            return new Content().Set(cdataText);
        }

        internal Content Set(string cdataText)
        {
            base.Set(cdataText);
            return this;
        }
    }
}
