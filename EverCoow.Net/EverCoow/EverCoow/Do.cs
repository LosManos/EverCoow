using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EverCoow.Extensions;
using System.Diagnostics;
using System.Xml;

namespace EverCoow
{
    public class Do
    {
        /// <summary>These are the placeholder strings.
        /// Placeholders are the texts to replace.
        /// </summary>
        public struct Placeholders
        {
            public string Leader; //  {{Leader}}
            public string ChapterHeader; //   {{ChapterHeader}}
            public string ArticleHeader;  //  {{ArticleHeader}}
            public string ArticleBody;    //  {{ArticleBody}}
        }

        /// <summary>This getter returns the default placeholders.
        /// </summary>
        public static EverCoow.Do.Placeholders DefaultPlaceholders
        {
            get
            {
                return new Do.Placeholders()
            {
                Leader = "{{Leader}}",
                ChapterHeader = "{{ChapterHeader}}",
                ArticleHeader = "{{ArticleHeader}}",
                ArticleBody = "{{ArticleBody}}"
            };
            }
        }


        /// <summary>This method takes data for an email template, names and files for the chapters and path and file for the resulting output.
        /// It merges the template with the chapters and writes the output file.
        /// </summary>
        /// <param name="templatePath">Path to the template</param>
        /// <param name="templateFilename">Name of the template file.</param>
        /// <param name="enexLeaderPath"></param>
        /// <param name="enexLeaderFilename"></param>
        /// <param name="enexChapterPath">Path to the .enex chapter files.</param>
        /// <param name="enexChapterList">List of chapter names and their respective file names.</param>
        /// <param name="placeholders">The placeholders we use for replacing text.  Typically Do.DefaultPlaceholders.</param>
        /// <param name="markdownConverter">Converter for enexmarkdown to emailhtml.</param>
        /// <param name="outPath">Path of the resulting output file.</param>
        /// <param name="outFilename">The name of the resulting output file.</param>
        public void Convert(
            string templatePath, string templateFilename,
            string enexLeaderPath, string enexLeaderFilename, 
            string enexChapterPath, List<EnexChapter> enexChapterList, 
            Placeholders placeholders, 
            IMarkDownConverter markdownConverter, 
            string outPath, string outFilename)
        {
            var emailTemplate = ReadTextOfFile(templatePath, templateFilename);
            var template = Split(emailTemplate, placeholders);

            var sb = new StringBuilder();

            sb.Append(template.Before);
            sb.Append(template.Leader.Before);
            if (null == enexLeaderPath)
            {
                //  We have no info about the Leader so keep the template Leader.
                sb.Append(template.Leader.Placeholder);
            }
            else
            {
                var leaderEnex = new Enex();
                var articles = leaderEnex.Read(enexLeaderPath, enexLeaderFilename);
                var leaderArticle = articles.Single();
                sb.Append(leaderArticle.CdataContent.MailText);
            }
            sb.Append(template.Leader.After);

            if (null == enexChapterPath)
            {
                //  We have no info about the Chapters and Articles so keep the template data.
                sb.Append(template.ChapterHeader.ToString());
                sb.Append(template.Article.ToString());
            }
            else
            {
                var enex = new EverCoow.Enex();
                foreach (var chapter in enexChapterList)
                {
                    sb.Append(template.ChapterHeader.WithPlaceholder(chapter.ChapterName));

                    var articleList = enex.Read(enexChapterPath, chapter.FileName);
                    foreach (var article in articleList)
                    {
                        var articleDoc = new XmlDocument();
                        articleDoc.LoadXml(article.CdataContent.InnerXml.InnerXml);
                        var articleString = markdownConverter.Convert(articleDoc);
                        sb.Append(template.Article.WithPlaceholder(article.Title, articleString));
                    }
                }
            }

            sb.Append(template.After);

            WriteFile(outPath, outFilename, sb);
        }

        private static TemplateStruct Split(string emailTemplate, Placeholders placeholders)
        {
            //  Split template into Before-Leader and Leader-and-after.
            var tuple = emailTemplate.SplitAt(@"<tr mc:repeatable>");
            var beforeLeader = tuple.Item1;

            var leader = @"<tr mc:repeatable>";
            tuple = tuple.Item2.SplitAt("<tr mc:repeatable>");
            leader += tuple.Item1;

            var chapterHeader = "<tr mc:repeatable>";
            tuple = tuple.Item2.SplitAt("<tr mc:repeatable>");
            chapterHeader += tuple.Item1;

            var article = "<tr mc:repeatable>";
            tuple = tuple.Item2.SplitAt("</tr>");
            article += tuple.Item1 + "</tr>";
            tuple = tuple.Item2.SplitAt("</tr>");
            article += tuple.Item1 + "</tr>";

            var footer = tuple.Item2;

            var ret = new TemplateStruct()
            {
                Before = beforeLeader,
                Leader = TemplateStruct.PlaceholderStruct.Create( leader, placeholders.Leader), 
                ChapterHeader = TemplateStruct.PlaceholderStruct.Create( chapterHeader, placeholders.ChapterHeader), 
                Article = TemplateStruct.PlaceholderStruct2.Create(article, placeholders.ArticleHeader, placeholders.ArticleBody),
                After = footer
            };

            return ret;
        }

        //private static TemplateStruct Split3(string emailTemplate)
        //{
        //    const string BeforePattern = @"(.*?)<tr mc:repeatable>";
        //    var matches = Regex.Matches(emailTemplate, BeforePattern, RegexOptions.Singleline);
        //    var beforeText = matches[0].Groups[1].Value;

        //    string LeaderPattern = @"(.*?)(<tr mc:repeatable>.*?{{ledare}}.*?<\/tr>)";
        //    var xmatches = Regex.Match(emailTemplate, LeaderPattern, RegexOptions.Singleline);
        //    var leaderBefore = matches[0].Groups[1].Value;
        //    var leaderPlaceholder = matches[0].Groups[2].Value;
        //    var leaderAfter = matches[0].Groups[3].Value;

        //    return new TemplateStruct();
        //}

        //private static TemplateStruct Split2(string emailTemplate)
        //{
        //    //(<tr mc:repeatable>.*?({{ledare}}).*?<\/tr>).*(<tr mc:repeatable>.*?({{ChapterHeader.*}}).*?<\/tr>).*(<tr mc:repeatable>.*?({{ArticleHeader}}).*?({{ArticleBody.*}}).*?<\/tr>)
        //    const string Pattern = @"(.*?)(<tr mc:repeatable>.*?)({{ledare}})(.*?<\/tr>)(.*)(<tr mc:repeatable>.*?)({{ChapterHeader.*}})(.*?<\/tr>)(.*)(<tr mc:repeatable>.*?)({{ArticleHeader}})(.*?)({{ArticleBody.*}})(.*?<\/tr>)(.*)";
        //    var matches = Regex.Matches(emailTemplate, Pattern, RegexOptions.Singleline);
        //    var match = matches[0];
        //    return new TemplateStruct()
        //    {
        //        Before = match.Groups[1].ToString(),
        //        Leader = new TemplateStruct.PlaceholderStruct()
        //        {
        //            Before = match.Groups[2].ToString(),
        //            Placeholder = match.Groups[3].ToString(),
        //            After = match.Groups[4].ToString(),
        //        },
        //        BetweenLeaderAndFirstChapter = match.Groups[5].ToString(),
        //        ChapterHeader = new TemplateStruct.PlaceholderStruct()
        //        {
        //            Before = match.Groups[6].ToString(),
        //            Placeholder = match.Groups[7].ToString(),
        //            After = match.Groups[8].ToString()
        //        },
        //        ArticleHeader = new TemplateStruct.PlaceholderStruct()
        //        {
        //            Before = match.Groups[9].ToString(),
        //            Placeholder = match.Groups[10].ToString(),
        //            After = match.Groups[11].ToString()
        //        },
        //        Article = new TemplateStruct.PlaceholderStruct()
        //        {
        //            Before = match.Groups[12].ToString(),
        //            Placeholder = match.Groups[13].ToString(),
        //            After = match.Groups[14].ToString()
        //        },
        //        After = match.Groups[15].ToString()
        //    };
        //}

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

        /// <summary>This method writes the strinbuilder parameter as a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="sb"></param>
        private void WriteFile(string path, string filename, StringBuilder sb)
        {
            File.WriteAllText( Path.Combine( path, filename), sb.ToString() );
        }

    }
}
