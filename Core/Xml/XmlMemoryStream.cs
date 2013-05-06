using System.IO;
using System.Text;
using System.Xml.Linq;

namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// Loads and saves XmlData to a memory stream
	/// </summary>
	public class XmlMemoryStream
	{
		public XmlMemoryStream(XmlData xmlData)
		{
			Root = xmlData;
		}

		public XmlData Root { get; private set; }

		public XmlMemoryStream(Stream stream)
		{
			XDocument xDoc = XDocument.Load(stream);
			Root = new XmlData(xDoc.Root);
		}

		public MemoryStream Save()
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream, new UTF8Encoding(false));
			Root.XRootElement.Document.Save(writer);
			writer.Write(writer.NewLine);
			return stream;
		}
	}
}