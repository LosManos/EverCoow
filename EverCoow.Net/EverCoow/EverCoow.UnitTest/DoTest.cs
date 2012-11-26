using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace EverCoow.UnitTest
{
    [TestClass]
    public class DoTest
    {
        private readonly EverCoow.Do.Placeholders placeholders = new Do.Placeholders(){
            Leader=  "{{Leader}}", 
            ChapterHeader=   "{{ChapterHeader}}", 
            ArticleHeader ="{{ArticleHeader}}", 
            ArticleBody =  "{{ArticleBody}}"
    };

        [TestMethod]
        public void CopyTemplateTest()
        {
            const string InFilename = "Template.Data.html";
            const string OutFilename = "CopyTemplateTest.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), InFilename, 
                null, null, null, null,
                placeholders, 
                GetDataPath(), OutFilename);

            var originalTemplate = ReadTextOfFile(GetDataPath(), InFilename);
            var createdEmail = ReadTextOfFile(GetDataPath(), OutFilename);
            Assert.AreEqual(originalTemplate, createdEmail, "Copying a template should render an equal result.");
        }

        [TestMethod]
        public void SimpleCopyTemplateTest()
        {
            const string InFilename = "SimplifiedTemplate.Data.html";
            const string OutFilename = "SimpleCopyTemplateTest.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), InFilename,
                null, null, null, null,
                placeholders,
                GetDataPath(), OutFilename);

            var originalTemplate = ReadTextOfFile(GetDataPath(), InFilename);
            var createdEmail = ReadTextOfFile(GetDataPath(), OutFilename);
            Assert.AreEqual(originalTemplate, createdEmail, "Copying a template should render an equal result.");
        }

        [TestMethod]
        public void ReplacePlaceholdersWithIdenticalData()
        {
            const string TemplateFilename = "Template.Data.html";
            const string LeaderFilename = "LeaderAsPlaceholder.enex";
            const string ArticleFilename = "ArticleAsPlaceholder.enex";
            const string OutFileName = "ReplacePlaceholdersWithIdenticalData.out.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), TemplateFilename,
                GetDataPath(), LeaderFilename,
                GetDataPath(), new List<EnexChapter>() { EnexChapter.Create("{{ChapterHeader}}", ArticleFilename) },
                placeholders,
                GetDataPath(), OutFileName);

            Assert.AreEqual(ReadTextOfFile( GetDataPath(), TemplateFilename), ReadTextOfFile(GetDataPath(), OutFileName), "Replacing place holders with the same content should render an equal result.");
        }

        [TestMethod]
        public void ProperMerge()
        {
            const string TemplateFilename = "Template.Data.html";
            const string LeaderFilename = "MyFirstLeader.enex";
            const string Chapter1Name = "My first chapter";
            const string Chapter1ArticlesFilename = "MyFirstArticleChapter1.enex";
            const string Chapter2Name = "My second chapter";
            const string Chapter2ArticlesFilename = "MyArticlesChapter2.enex";
            const string OutFileName = "ProperMerge.out.html";

            var testeen = new EverCoow.Do();
            testeen.Convert( GetDataPath(), TemplateFilename, 
                GetDataPath(), LeaderFilename, 
                GetDataPath(), new List<EnexChapter>(){
                    EnexChapter.Create( Chapter1Name, Chapter1ArticlesFilename ), 
                    EnexChapter.Create( Chapter2Name, Chapter2ArticlesFilename )},
                placeholders,
                GetDataPath(), OutFileName);

            Assert.Inconclusive("Yet not implemented.  This should be a full fledged test.");
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
            using (var sr = File.OpenText(Path.Combine(path, filename)))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
