using System.Collections.Generic;
using System.IO;

namespace EverCoow.Console
{
    class Program
    {
        private const string TemplatePath = @"Z:\Documents\Development\Projects\EverCoow\EverCoow.Net\EverCoow\Data";

        private static readonly Do.Placeholders Placeholders = new Do.Placeholders()
        {
            Leader = "{{Leader}}",
            ChapterHeader = "{{ChapterHeader}}",
            ArticleHeader = "{{ArticleHeader}}",
            ArticleBody = "{{ArticleBody}}"
        };
        
        static void Main(string[] args)
        {
            System.Console.WriteLine("Converting...");
            var chapters = new List<EnexChapter>() {
                EverCoow.EnexChapter.Create( "Code and development", "mb-1-CodeAndDevelopment"), 
                EverCoow.EnexChapter.Create( "Projects and leadership", "mb-2-ProjectsAndLeadership"), 
                EverCoow.EnexChapter.Create( "Products and releases", "mb-3-ProductsAndReleases"), 
                EverCoow.EnexChapter.Create("Privacy, security and rignts", "mb-4-PrivacySecurityAndRights" ),
                EverCoow.EnexChapter.Create( "Miscellaneous", "mb-5-Miscellaneous")
            };
            const string dataPath = @"\\psf\Home\Documents\Development\Projects\EverCoow\EverCoow.Net\EverCoow\Data";
            const string enexLeaderNotebookName = "Ledaridéer";

            var evercoowDo = new EverCoow.Do();
            evercoowDo.Convert(
                TemplatePath, "template.html", 
                dataPath, enexLeaderNotebookName, chapters, Placeholders, new EverCoowMarkDownConverter(), 
                dataPath, "email.html");

            System.Console.WriteLine("Conversion finished.");
            System.Console.Write("Press any key...");
            System.Console.ReadKey();
        }





    }
}
