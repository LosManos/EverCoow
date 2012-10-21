using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EverCoow
{
    public class Enex
    {
        public IList<Article> Read(string pathFilename)
        {
            var doc = new XmlDocument();
            doc.Load(pathFilename );

            var ret = new List<Article>();
            foreach (XmlNode node in doc.SelectNodes("//note[tag[contains(.,'mbb')]]"))
            {
                ret.Add(Article.Create(node));
            }

            return ret;
            //var q = ((IEnumerable<XmlNode>)doc.SelectNodes("//note[tag[contains(.,'mbb')]]"))
            //    .Select(node => Article.Create(node));

            //return q.ToList();
        }
    }
}
