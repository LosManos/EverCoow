using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EverCoow
{
    /// <summary>This interface sets the methods for  the Enex-Markdown to Email-HTML converter.
    /// </summary>
    public interface IMarkDownConverter
    {
        string Convert(XmlDocument enexDoc);
    }

    public interface IMarkDownUnitTest
    {
        string ConvertLink(string enexRow);
    }
}
