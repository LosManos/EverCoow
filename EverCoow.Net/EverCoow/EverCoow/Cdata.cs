using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace EverCoow
{
    public class Cdata
    {
        private string _cdataText;

        public string CdataText { get { return _cdataText; } }

        public string Value
        {
            get
            {
                //<![CDATA[<?xml version="1.0" encoding="UTF-8" standalone="no"?>
                //<!DOCTYPE en-note SYSTEM "http://xml.evernote.com/pub/enml2.dtd">
                //<en-note sty...
                //...iv>\n</en-note>\n]]>
                var matches = Regex.Matches(_cdataText, @"^<!\[CDATA.+\?>\s+(.*)\s+\]\]>", RegexOptions.Singleline);
                return matches[0].Groups[1].Value;
            }
        }

        public XmlDocument InnerXml
        {
            get
            {
                //<!DOCTYPE en-note SYSTEM "http://xml.evernote.com/pub/enml2.dtd">
                //<en-note styl...>
                //...
                //</en-note>
                var matches = Regex.Matches( Value, @"<!DOCTYPE.+>.*(<en-note.+<\/en-note>).*", RegexOptions.Singleline );
                var s = matches[0].Groups[1].Value;
                var doc = new XmlDocument();
                doc.LoadXml(s);
                return doc;
            }
        }

        internal static Cdata Create(string cdataText)
        {
            return new Cdata().Set(cdataText);
        }

        internal Cdata Set(string cdataText)
        {
            this._cdataText = cdataText;
            return this;
        }
    }
}
