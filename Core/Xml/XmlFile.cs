using System.IO;
using System.Text;
using System.Xml.Linq;

namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// Loads and saves XmlData to file
	/// </summary>
	public class XmlFile
	{
		public XmlFile(XmlData xmlData)
		{
			Root = xmlData;
		}

		public XmlData Root { get; private set; }

		public XmlFile(string filePath)
		{
			using (
				var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				Root = new XmlData(XDocument.Load(stream).Root);
		}

		public void Save(string filePath)
		{
			using (
				var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
					FileShare.ReadWrite))
				SaveDocumentToStream(stream);
		}

		private void SaveDocumentToStream(Stream stream)
		{
			using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
				SaveDocument(writer);
		}

		private void SaveDocument(TextWriter writer)
		{
			Root.XRootElement.Document.Save(writer);
			writer.Write(writer.NewLine);
			writer.Flush();
		}
	}
}