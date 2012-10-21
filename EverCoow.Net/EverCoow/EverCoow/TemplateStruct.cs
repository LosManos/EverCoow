using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverCoow
{
    internal struct TemplateStruct
    {
        internal struct PlaceholderStruct
        {
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
}
