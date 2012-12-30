using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EverCoow.UnitTest
{
    [TestClass]
    public class MarkDownTest
    {
        [TestMethod]
        public void ConvertMDUrl() 
        {
            var xmlString = 
            "<en-note style='word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;'>" +
                "<div>- http://example.com/\t\t&lt;- with comment</div>" +
                "</en-note>";
            var expRes =
                "- <a href=\"http://example.com/\" target=\"_blank\">http://example.com/</a>\t\t&lt;- with comment";

            var enexDoc = new XmlDocument();
            enexDoc.LoadXml(xmlString);

            var md = new EverCoowMarkDownConverter();
            var res = md.Convert(enexDoc);

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void ConvertArticle()
        {
            var xmlString =
                "<en-note style='word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;'>" +
                "Eating cows is like" +
                "<div>eating a part of your</div>" +
                "<div>soul.</div>" +
                "<div><br/></div>" +
                "<div>- http://example.com/\t\t&lt;- with comment</div>" +
                "<div>- http://example2.com/withoutcomment.aspx?id=42</div>" +
                "</en-note>";
            var expRes =
                "Eating cows is like<br/>" + Environment.NewLine +
                "eating a part of your<br/>" + Environment.NewLine +
                "soul.<br/>" + Environment.NewLine + "<br/>" + Environment.NewLine +
                "- <a href=\"http://example.com/\" target=\"_blank\">http://example.com/</a>\t\t&lt;- with comment<br/>" + Environment.NewLine  +
                "- <a href=\"http://example2.com/withoutcomment.aspx?id=42\" target=\"_blank\">http://example2.com/withoutcomment.aspx?id=42</a>";

            var enexDoc = new XmlDocument();
            enexDoc.LoadXml(xmlString);

            var md = new EverCoowMarkDownConverter();
            var res = md.Convert(enexDoc);

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void ConvertSimpleText()
        {
            var xmlString =
                @"<en-note style='word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;'>" +
                "Just text" +
                "<div>Text in div</div>" +
                "<div><a href='http://simple.example.com'>simple link</a></div>" +
                "<div><a href='http://www.example.com/a/page.aspx?a=1&amp;b=2'>buggy complex link</a></div>" +
                "</en-note>";
            var expRes =
                "Just text" +
                "<br/>" + Environment.NewLine + 
                "Text in div" +
                "<br/>" + Environment.NewLine + 
                "<a href=\"http://simple.example.com\" target=\"_blank\">simple link</a>" +
                "<br/>" + Environment.NewLine + 
                "<a href=\"http://www.example.com/a/page.aspx?a=1&b=2\" target=\"_blank\">buggy complex link</a>";

            var enexDoc = new XmlDocument();
            enexDoc.LoadXml(xmlString);

            var md = new EverCoowMarkDownConverter();
            var res = md.Convert(enexDoc);

            Assert.AreEqual(expRes, res);


            enexDoc.LoadXml(xmlString);
        }

        [TestMethod]
        public void ConvertArticleLeader()
        {
            var xmlString =
                @"<en-note style='word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;'>" +
                "This is my leader text." +
                "<div><br/></div>" +
                "<div>With some more info.  And another sentence.</div>" +
                "<div><br/></div>" +
                "<div>With one <a href='http://link.example.com/a/&amp;x=1?y=2'>link</a> to start.  And <a href='http://another.example.com'>another</a>.</div>" +
                "</en-note>";  
            var expRes =
                "This is my leader text.<br/>" + Environment.NewLine +
                "<br/>" + Environment.NewLine +
                "With some more info.  And another sentence.<br/>" + Environment.NewLine +
                "<br/>" + Environment.NewLine + 
                "With one <a href=\"http://link.example.com/a/&x=1?y=2\" target=\"_blank\">link</a> to start.  " +
                "And <a href=\"http://another.example.com\" target=\"_blank\">another</a>.";

            var enexDoc = new XmlDocument();
            enexDoc.LoadXml(xmlString);

            var md = new EverCoowMarkDownConverter();
            var res = md.Convert(enexDoc);

            Assert.AreEqual(expRes, res);


            enexDoc.LoadXml(xmlString);
        }


        [TestMethod]
        public void ConvertExplicitEverCoowMarkDownLink()
        {
            IMarkDownUnitTest testee = new EverCoowMarkDownConverter();
            var res = testee.ConvertMDECLink("- http://example.com");
            Assert.AreEqual("- <a href=\"http://example.com\" target=\"_blank\">http://example.com</a>", res);

            res = testee.ConvertMDECLink("- http://example.com/");
            Assert.AreEqual("- <a href=\"http://example.com/\" target=\"_blank\">http://example.com/</a>", res);

            res = testee.ConvertMDECLink("- http://example.com/myfile.aspx");
            Assert.AreEqual("- <a href=\"http://example.com/myfile.aspx\" target=\"_blank\">http://example.com/myfile.aspx</a>", res);

            res = testee.ConvertMDECLink("- http://example.com  &lt;-my comment");
            Assert.AreEqual("- <a href=\"http://example.com\" target=\"_blank\">http://example.com</a>\t\t&lt;- my comment", res);

            res = testee.ConvertMDECLink("- http://example.com/     &lt;-my comment");
            Assert.AreEqual("- <a href=\"http://example.com/\" target=\"_blank\">http://example.com/</a>\t\t&lt;- my comment", res);

            res = testee.ConvertMDECLink("- http://example.com/myfile.aspx  &lt;- my comment");
            Assert.AreEqual("- <a href=\"http://example.com/myfile.aspx\" target=\"_blank\">http://example.com/myfile.aspx</a>\t\t&lt;- my comment", res);
        }

        [TestMethod]
        public void ConvertImplicitEverCoowMarkDownLink()
        {
            IMarkDownUnitTest testee = new EverCoowMarkDownConverter();
            var res = testee.ConvertMDECLink("http://example.com");
            Assert.AreEqual("<a href=\"http://example.com\" target=\"_blank\">http://example.com</a>", res);

            res = testee.ConvertMDECLink("http://example.com/");
            Assert.AreEqual("<a href=\"http://example.com/\" target=\"_blank\">http://example.com/</a>", res);

            res = testee.ConvertMDECLink("Some text before http://example.com/myfile.aspx");
            Assert.AreEqual("Some text before <a href=\"http://example.com/myfile.aspx\" target=\"_blank\">http://example.com/myfile.aspx</a>", res);

            res = testee.ConvertMDECLink("http://example.com some text after.");
            Assert.AreEqual("<a href=\"http://example.com\" target=\"_blank\">http://example.com</a> some text after.", res);

            res = testee.ConvertMDECLink("Some text before http://example.com/ and after");
            Assert.AreEqual("Some text before <a href=\"http://example.com/\" target=\"_blank\">http://example.com/</a> and after", res);
        }

    }
}
