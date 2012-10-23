using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace EverCoow.UnitTest
{
    [TestClass]
    public class DoTest
    {
        [TestMethod]
        public void CopyTemplateTest()
        {
            const string InFilename = "Template.Data.html";
            const string OutFilename = "CopyTemplateTest.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), InFilename, 
                null, null, null, null,
                GetDataPath(), OutFilename);

            var originalTemplate = ReadTextOfFile(GetDataPath(), InFilename);
            var createdEmail = ReadTextOfFile(GetDataPath(), OutFilename);
            Assert.AreEqual(originalTemplate, createdEmail, "Copying a template should render an equal result.");
        }

        [TestMethod]
        public void SimpleCopyTemplateTest()
        {
            const string InFilename = "SimplifiedTemplate.Data.html";
            const string OutFilename = "CopySimpleTemplateTest.html";

            var testee = new EverCoow.Do();
            testee.Convert(GetDataPath(), InFilename,
                null, null, null, null,
                GetDataPath(), OutFilename);

            var originalTemplate = ReadTextOfFile(GetDataPath(), InFilename);
            var createdEmail = ReadTextOfFile(GetDataPath(), OutFilename);
            Assert.AreEqual(originalTemplate, createdEmail, "Copying a template should render an equal result.");
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

        private static string ReadTextOfFile(string path, string filename)
        {
            using (var sr = File.OpenText(Path.Combine(path, filename)))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
