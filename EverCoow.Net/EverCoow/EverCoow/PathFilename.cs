namespace EverCoow
{
    internal class PathFilename
    {
        public string Path { get; private set; }
        public string Filename { get; private set; }
        public string Full{
            get { return System.IO.Path.Combine(Path, Filename); }
        }

        public static PathFilename Create(string path, string filename)
        {
            return new PathFilename().Set(path, filename);
        }
        private PathFilename Set(string path, string filename)
        {
           this.Path = path;
            this.Filename = filename;
            return this;
        }
    }
}
