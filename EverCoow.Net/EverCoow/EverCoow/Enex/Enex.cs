using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace EverCoow.Enex 
{
    internal class Enex : IEnex
    {
        private readonly XmlDocument _doc;

        internal Enex()
        {
            _doc = new XmlDocument();
        }

        void IEnex.Load(string pathFilename)
        {
            _doc.Load(pathFilename);
        } 

        internal IEnumerable<Note> GetNotes()
        {
            return (
                        _doc.SelectNodes("note")
                        .Cast<XmlNode>()
                        .Select(Note.Create)).ToList();
        } 

        internal IList<Article> Read(string pathFilename)
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
        internal IList<Article> ReadFile(string path, string filename)
        {
            return Read(Path.Combine(path, filename));
        }
    }
}
