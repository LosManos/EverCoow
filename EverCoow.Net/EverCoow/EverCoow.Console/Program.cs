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
            var chapterList = new List<Tuple<string,string>>() {
                new Tuple<string,string>("Projects and leadersip", "mb-2-ProjectsAndLeadership.enex"), 
                new Tuple<string,string>("Privacy, security and rignts", "mb-4-PrivacySecurityAndRights.enex" )
            };
            const string path = @"\\psf\Home\Documents\Development\Projects\EverCoow\EverCoow.Net\EverCoow\Data";
            var emailTemplate = ReadTextOfFile( TemplatePath, "template.html");
            var template = Split(emailTemplate);
            var sb = new StringBuilder();

            sb.Append(template.Before);
            sb.Append(template.Leader.Before);
            sb.Append("NyLedare");
            sb.Append(template.Leader.After);
            
            sb.Append(template.BetweenLeaderAndFirstChapter);

            var enex = new EverCoow.Enex();
            foreach (var chapter in chapterList)
            {
                sb.Append(template.ArticleHeader.Before);
                sb.Append(chapter.Item1);
                sb.Append(template.ArticleHeader.After);
                
                var articleList = enex.Read(Path.Combine(path, chapter.Item2));
                foreach (var article in articleList)
                {
                    sb.Append(template.Article.Before);
                    sb.Append( article.Title );
                    sb.Append(article.CdataContent.MailText);
                    sb.Append(template.Article.After);
                }
            }

            sb.Append(template.After);
        }

        private struct TemplateStruct
        {
            internal struct PlaceholderStruct{
                internal string Before;
                internal string Placeholder;
                internal string After;
            };
            internal string Before;
            internal PlaceholderStruct Leader;
            internal string BetweenLeaderAndFirstChapter;
            internal PlaceholderStruct ChapterHeader;
            internal PlaceholderStruct ArticleHeader;
            internal PlaceholderStruct Article;
            internal string After;
        }

        private static TemplateStruct Split(string emailTemplate)
        {
            //(<tr mc:repeatable>.*?({{ledare}}).*?<\/tr>).*(<tr mc:repeatable>.*?({{ChapterHeader.*}}).*?<\/tr>).*(<tr mc:repeatable>.*?({{ArticleHeader}}).*?({{ArticleBody.*}}).*?<\/tr>)
            const string Pattern = @"(.*?)(<tr mc:repeatable>.*?)({{ledare}})(.*?<\/tr>)(.*)(<tr mc:repeatable>.*?)({{ChapterHeader.*}})(.*?<\/tr>)(.*)(<tr mc:repeatable>.*?)({{ArticleHeader}})(.*?)({{ArticleBody.*}})(.*?<\/tr>)(.*)";
            var matches = Regex.Matches(emailTemplate, Pattern, RegexOptions.Singleline);
            var match = matches[0];
            return new TemplateStruct()
            {
                Before = match.Groups[1].ToString(),
                Leader = new TemplateStruct.PlaceholderStruct()
                {
                    Before = match.Groups[2].ToString(), 
                    Placeholder = match.Groups[3].ToString(),
                    After = match.Groups[4].ToString(), 
                },
                BetweenLeaderAndFirstChapter = match.Groups[5].ToString(), 
                ChapterHeader = new TemplateStruct.PlaceholderStruct()
                {
                    Before = match.Groups[6].ToString(),
                    Placeholder = match.Groups[7].ToString(),
                    After = match.Groups[8].ToString()
                },
                ArticleHeader = new TemplateStruct.PlaceholderStruct()
                {
                    Before = match.Groups[9].ToString(),
                    Placeholder = match.Groups[10].ToString(),
                    After = match.Groups[11].ToString()
                },
                Article = new TemplateStruct.PlaceholderStruct()
                {
                    Before = match.Groups[12].ToString(),
                    Placeholder = match.Groups[13].ToString(),
                    After = match.Groups[14].ToString()
                },
                After = match.Groups[15].ToString()
            };
        }

        private static string ReadTextOfFile( string path, string filename ){
            using (var sr = File.OpenText(Path.Combine(path, filename)))
            {
                return sr.ReadToEnd();
            }
        }

    }
}
