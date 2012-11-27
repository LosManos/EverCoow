using System.Collections.Generic;
using System.IO;

namespace EverCoow.Console
{
    class Program
    {
        private const string TemplatePath = @"Z:\Documents\Development\Projects\EverCoow\EverCoow.Net\EverCoow\Data";

        private static readonly EverCoow.Do.Placeholders Placeholders = new Do.Placeholders()
        {
            Leader = "{{Leader}}",
            ChapterHeader = "{{ChapterHeader}}",
            ArticleHeader = "{{ArticleHeader}}",
            ArticleBody = "{{ArticleBody}}"
        };
        
        static void Main(string[] args)
        {
            System.Console.WriteLine("Converting...");
            var chapterList = new List<EverCoow.EnexChapter>() {
                EverCoow.EnexChapter.Create( "Projects and leadersip", "mb-2-ProjectsAndLeadership.enex"), 
                EverCoow.EnexChapter.Create("Privacy, security and rignts", "mb-4-PrivacySecurityAndRights.enex" )
            };
            const string path = @"\\psf\Home\Documents\Development\Projects\EverCoow\EverCoow.Net\EverCoow\Data";

            var outPathFile = Path.Combine(path, "letter.html");

            var evercoowDo = new EverCoow.Do();
            evercoowDo.Convert(TemplatePath, "template.html", path, "", "", chapterList, Placeholders, new MarkDownConverter(), path, "email.html");

            System.Console.WriteLine("Conversion finished.");
            System.Console.Write("Press any key...");
            System.Console.ReadKey();
        }





    }
}
