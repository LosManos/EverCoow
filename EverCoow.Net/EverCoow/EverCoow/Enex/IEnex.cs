namespace EverCoow.Enex
{
    internal interface IEnex
    {
        void Load(string pathFilename);

        IEnex<Note> GetNotes();
    }
}
