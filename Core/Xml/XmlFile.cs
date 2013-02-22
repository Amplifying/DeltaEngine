using System.IO;

namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// This handles the saving and loading of XmlData objects
	/// </summary>
	public class XmlFile
	{
		public XmlFile(XmlData xmlData = null)
		{
			Root = xmlData;
		}

		public XmlData Root { get; private set; }

		public XmlFile(string filename)
		{
			Root = new XmlTranscriber(File.ReadAllText(filename)).Result;
		}

		public void Save(string filename)
		{
			File.WriteAllText(filename, Root.ToXmlString());
		}
	}
}