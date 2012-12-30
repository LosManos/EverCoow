using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace EverCoow.UnitTest
{
    [TestClass]
    public class DoTest
    {
    //    private readonly EverCoow.Do.Placeholders placeholders = new Do.Placeholders(){
    //        Leader=  "{{Leader}}", 
    //        ChapterHeader=   "{{ChapterHeader}}", 
    //        ArticleHeader ="{{ArticleHeader}}", 
    //        ArticleBody =  "{{ArticleBody}}"
    //};

        [TestMethod]
        public void CopyTemplateTest()
        {
            const string inFilename = "Template.Data.html";
            const string outFilename = "CopyTemplateTest.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), inFilename, 
                null, null, null, null,
                Do.DefaultPlaceholders, 
                new EverCoowMarkDownConverter(), 
                GetDataPath(), outFilename);

            var originalTemplate = ReadTextOfFile(GetDataPath(), inFilename);
            var createdEmail = ReadTextOfFile(GetDataPath(), outFilename);
            Assert.AreEqual(originalTemplate, createdEmail, "Copying a template should render an equal result.");
        }

        [TestMethod]
        public void SimpleCopyTemplateTest()
        {
            const string inFilename = "SimplifiedTemplate.Exp.html";
            const string outFilename = "SimpleCopyTemplateTest.Out.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), inFilename,
                null, null, null, null,
                Do.DefaultPlaceholders,
                new EverCoowMarkDownConverter(), 
                GetDataPath(), outFilename);

            var originalTemplate = ReadTextOfFile(GetDataPath(), inFilename);
            var createdEmail = ReadTextOfFile(GetDataPath(), outFilename);
            Assert.AreEqual(originalTemplate, createdEmail, "Copying a template should render an equal result.");
        }

        [TestMethod]
        public void ReplacePlaceholdersWithIdenticalData()
        {
            const string templateFilename = "Template.Data.html";
            const string leaderFilename = "LeaderAsPlaceholder.enex";
            const string articleFilename = "ArticleAsPlaceholder.enex";
            const string outFileName = "ReplacePlaceholdersWithIdenticalData.out.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), templateFilename,
                GetDataPath(), leaderFilename,
                GetDataPath(), new List<EnexChapter>() { EnexChapter.Create("{{ChapterHeader}}", articleFilename) },
                Do.DefaultPlaceholders,
                new EverCoowMarkDownConverter(), 
                GetDataPath(), outFileName);

            Assert.AreEqual(
                ReadTextOfFile( GetDataPath(), templateFilename), 
                ReadTextOfFile(GetDataPath(), outFileName), 
                "Replacing place holders with the same content should render an equal result.");
        }

        [TestMethod]
        public void ProperMerge()
        {
            const string templateFilename = "Template.Data.html";
        
            //<en-note style="word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;">
            //hejdig.
            //<div><br/></div>
            //<div>This is my leader text.</div>
            //<div><br/></div>
            //<div>With a <a href="http://www.example.com">stray link</a>.</div>
            //<div><br/></div>
            //<div>/OF</div>
            //<div><br/></div>
            //<div>----8&lt;----</div>
            //</en-note>

            const string leaderFilename = "MyFirstLeader.enex";
            const string chapter1Name = "My first chapter";
            const string chapter1ArticlesFilename = "MyFirstArticleChapter1.enex";
            const string chapter2Name = "My second chapter";
            const string chapter2ArticlesFilename = "MyArticlesChapter2.enex";
            const string outFilename = "ProperMerge.out.html";
            const string expectedOutFilename = "ProperMerge.exp.html";

            var testee = new EverCoow.Do();
            testee.Convert( GetDataPath(), templateFilename, 
                GetDataPath(), leaderFilename, 
                GetDataPath(), new List<EnexChapter>(){
                    EnexChapter.Create( chapter1Name, chapter1ArticlesFilename ), 
                    EnexChapter.Create( chapter2Name, chapter2ArticlesFilename )},
                    Do.DefaultPlaceholders,
                    new EverCoowMarkDownConverter(), 
                GetDataPath(), outFilename);

            var outData = ReadTextOfFile(GetDataPath(), outFilename);
            var expData = ReadTextOfFile(GetDataPath(), expectedOutFilename);
            Assert.AreEqual(expData, outData);
        }

        private static string GetDataPath()
        {
            //  http://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
            //get the full location of the assembly with DaoTests in it
            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(EverCoow.Do)).Location;

            //get the folder that's in
            string theDirectory = Path.GetDirectoryName(fullPath) + @"\..\..\Data";

            return theDirectory;
        }

        /// <summary>This method reads a text document and returns its contents as a string.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string ReadTextOfFile(string path, string filename)
        {
            string ret = null;
            using (var sr = File.OpenText(Path.Combine(path, filename)))
            {
                ret = sr.ReadToEnd();
            }
            return WashForLineEndings(ret);
        }

        /// <summary>This method does a very rude line endings wash.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string WashForLineEndings(string text)
        {
            return text.Replace(Environment.NewLine, "\n");
        }
    }
}
