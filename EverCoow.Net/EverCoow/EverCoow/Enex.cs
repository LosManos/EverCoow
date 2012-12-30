using System.Collections.Generic;
using System.IO;
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
// ReSharper disable PossibleNullReferenceException
            foreach (XmlNode node in doc.SelectNodes( "//note"))    //  "//note[tag[contains(.,'mbb')]]"))
// ReSharper restore PossibleNullReferenceException
            {
                ret.Add(Article.Create(node));
            }

            return ret;
            //var q = ((IEnumerable<XmlNode>)doc.SelectNodes("//note[tag[contains(.,'mbb')]]"))
            //    .Select(node => Article.Create(node));

            //return q.ToList();
        }

        /// <summary>This emthod reads the file in the parameters
        /// and returns its contents.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public IList<Article> ReadFile(string path, string filename)
        {
            return Read(Path.Combine(path, filename));
        }
    }
}
