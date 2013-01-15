using System.Xml;

namespace EverCoow.Enex
{
    internal class Note
    {
        private XmlNode _node;

        private  Note(){}

        internal static Note Create(XmlNode node)
        {
            var ret = new Note {_node = node};
            return ret;
        }
    }
}
