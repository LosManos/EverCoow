using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverCoow
{
    public class EnexChapter
    {
        public string ChapterName { get; private set; }
        public string FileName { get; private set; }
        public static EnexChapter Create(string chapterName, string fileName)
        {
            return new EnexChapter().Set(chapterName, fileName);
        }
        private EnexChapter Set(string chapterName, string fileName)
        {
            this.ChapterName = chapterName;
            this.FileName = fileName;
            return this;
        }
    }
}
