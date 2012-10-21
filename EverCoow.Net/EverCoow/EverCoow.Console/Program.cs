using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EverCoow.Console
{
    class Program
    {
        private const string TemplatePath = @"Z:\Documents\Development\Projects\EverCoow\EverCoow.Net\EverCoow\Data";
        static void Main(string[] args)
        {
            System.Console.WriteLine("Converting...");
            var chapterList = new List<EverCoow.EnexChapter>() {
                EverCoow.EnexChapter.Create( "Projects and leadersip", "mb-2-ProjectsAndLeadership.enex"), 
                EverCoow.EnexChapter.Create("Privacy, security and rignts", "mb-4-PrivacySecurityAndRights.enex" )
            };
            const string path = @"\\psf\Home\Documents\Development\Projects\EverCoow\EverCoow.Net\EverCoow\Data";

            var evercoowDo = new EverCoow.Do();
            evercoowDo.Convert(TemplatePath, "template.html", path, chapterList);

            System.Console.WriteLine("Conversion finished.");
            System.Console.Write("Press any key...");
            System.Console.ReadKey();
        }





    }
}
