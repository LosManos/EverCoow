using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EverCoow.Extensions;


namespace EverCoow
{
    internal struct TemplateStruct
    {
        internal struct PlaceholderStruct
        {
            internal string Before;
            internal string Placeholder;
            internal string After;
            internal static PlaceholderStruct Create(string text, string placeholder)
            {
                var tuple = text.SplitAt( placeholder);
                var ret = new PlaceholderStruct().Set(tuple.Item1, placeholder, tuple.Item2);
                return ret;
            }
            internal PlaceholderStruct Set(string before, string placeholder, string after)
            {
                this.Before = before;
                this.Placeholder = placeholder;
                this.After = after;
                return this;
            }
            public override string ToString()
            {
                return Before + Placeholder + After;
            }
            internal string WithPlaceholder(string placeholder)
            {
                return Before + placeholder + After;
            }
        };
        internal struct PlaceholderStruct2
        {
            internal string Before;
            internal string Placeholder1;
            internal string Between;
            internal string Placeholder2;
            internal string After;
            internal static PlaceholderStruct2 Create(string text, string placeholder1, string placeholder2)
            {
                var tuple1 = text.SplitAt( placeholder1);
                var tuple2 = tuple1.Item2.SplitAt( placeholder2 );
                var ret = new PlaceholderStruct2().Set(tuple1.Item1, placeholder1, tuple2.Item1, placeholder2, tuple2.Item2);
                return ret;
            }
            internal PlaceholderStruct2 Set(string before, string placeholder1, string between, string placeholder2, string after)
            {
                this.Before = before;
                this.Placeholder1 = placeholder1;
                this.Between = between;
                this.Placeholder2 = placeholder2;
                this.After = after;
                return this;
            }
            public override string ToString()
            {
                return Before + Placeholder1 + Between + Placeholder2 + After;
            }
            internal string WithPlaceholder(string placeholder1, string placeholder2)
            {
                return Before + placeholder1 + Between + placeholder2 + After;
            }
        };
        internal string Before;
        internal PlaceholderStruct Leader;
        internal PlaceholderStruct ChapterHeader;
        internal PlaceholderStruct2 Article;
        internal string After;
    }
}
