namespace EverCoow
{
    public class EnexChapter
    {
        /// <summary>This property is the name of the Chapter as presented as headline in the resulting email.
        /// </summary>
        public string ChapterName { get; private set; }

        /// <summary>This proeprty is the name of the Evernote Notebook that corresponds with this Chapter.
        /// </summary>
        public string NotebookName { get; private set; }

        /// <summary>This is the preferred constructor since it takes all necessary parameters.
        /// </summary>
        /// <param name="chapterName"></param>
        /// <param name="notebookName"></param>
        /// <returns></returns>
        public static EnexChapter Create(string chapterName, string notebookName)
        {
            return new EnexChapter().Set(chapterName, notebookName);
        }

        /// <summary>This helper method sets all necessary parameters for a full object.
        /// </summary>
        /// <param name="chapterName"></param>
        /// <param name="notebookName"></param>
        /// <returns></returns>
        private EnexChapter Set(string chapterName, string notebookName)
        {
            this.ChapterName = chapterName;
            this.NotebookName = notebookName;
            return this;
        }
    }
}
