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
        public void Convert()
        {
            var xmlString =
             @"
                <en-note style='word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;'>
                 Eating cows is like
                 <div>eating a part of your</div>
                 <div>soul.</div>
                 <div><br/></div>
                 <div>- http://example.com/     &lt;- with comment</div>
                 <div>- http://example2.com/withoutcomment.aspx?id=42</div>
                 </en-note>";
            var expRes =
                "Eating cows is like<br/>" +
                "eating a part of your<br/>" +
                "soul.<br/><br/>" +
                "<a href=\"http://example.com/\">http://example.com/</a>&nbsp;&nbsp;&lt;- with comment<br/>" +
                "<a href=\"http://example2.com/withoutcomment.aspx?id=42\">http://example2.com/withoutcomment.aspx?id=42</a>";

            var enexDoc = new XmlDocument();
            enexDoc.LoadXml(xmlString);

            var md = new MarkDownConverter();
            var res = md.Convert(enexDoc);

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void ConvertLink()
        {
            IMarkDownUnitTest testee = new MarkDownConverter();
            var res = testee.ConvertLink("- http://example.com");
            Assert.AreEqual("<a href=\"http://example.com\">http://example.com</a>", res);

            res = testee.ConvertLink("- http://example.com/");
            Assert.AreEqual("<a href=\"http://example.com/\">http://example.com/</a>", res);

            res = testee.ConvertLink("- http://example.com/myfile.aspx");
            Assert.AreEqual("<a href=\"http://example.com/myfile.aspx\">http://example.com/myfile.aspx</a>", res);

            res = testee.ConvertLink("- http://example.com  &lt;-my comment");
            Assert.AreEqual("<a href=\"http://example.com\">http://example.com</a>&nbsp;&nbsp;&lt;- my comment", res);

            res = testee.ConvertLink("- http://example.com/     &lt;-my comment");
            Assert.AreEqual("<a href=\"http://example.com/\">http://example.com/</a>&nbsp;&nbsp;&lt;- my comment", res);

            res = testee.ConvertLink("- http://example.com/myfile.aspx  &lt;- my comment");
            Assert.AreEqual("<a href=\"http://example.com/myfile.aspx\">http://example.com/myfile.aspx</a>&nbsp;&nbsp;&lt;- my comment", res);
        }
    }
}
