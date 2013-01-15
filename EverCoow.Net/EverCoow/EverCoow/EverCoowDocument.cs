using EverCoow.Enex;

namespace EverCoow
{
    internal class EverCoowDocument
    {
        private IEnex _enex;

        internal EverCoowDocument( IEnex enex)
        {
            _enex = enex;
        }

        internal LeaderArticle Leader{ 
            get
            {
                return _enex.GetNotes()        
            }
        }

        internal void Load(PathFilename pf)
        {
            _enex.Load(pf);
        }

    }
}
