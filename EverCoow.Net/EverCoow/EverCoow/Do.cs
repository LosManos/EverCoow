using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EverCoow
{
    public class Do
    {
        public void Convert(string templatePath, string templateName,
            string exechapterPath, List<EnexChapter> enexchapterList)
        {
            var emailTemplate = ReadTextOfFile(templatePath, templateName);
            var template = Split(emailTemplate);

            var sb = new StringBuilder();

            sb.Append(template.Before);
            sb.Append(template.Leader.Before);
            sb.Append("NyLedare");
            sb.Append(template.Leader.After);

            sb.Append(template.BetweenLeaderAndFirstChapter);

            var enex = new EverCoow.Enex();
            foreach (var chapter in enexchapterList)
            {
                sb.Append(template.ArticleHeader.Before);
                sb.Append(chapter.ChapterName);
                sb.Append(template.ArticleHeader.After);

                var articleList = enex.Read(Path.Combine(exechapterPath, chapter.FileName));
                foreach (var article in articleList)
                {
                    sb.Append(template.Article.Before);
                    sb.Append(article.Title);
                    sb.Append(article.CdataContent.MailText);
                    sb.Append(template.Article.After);
                }
            }

            sb.Append(template.After);
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

        private static string ReadTextOfFile(string path, string filename)
        {
            using (var sr = File.OpenText(Path.Combine(path, filename)))
            {
                return sr.ReadToEnd();
            }
        }

    }
}
