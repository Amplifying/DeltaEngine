using DeltaEngine.Core.Xml;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Implements XmlContent by loading XmlData from a file
	/// </summary>
	public sealed class XmlContentFile : XmlContent
	{
		public XmlContentFile(string contentFilename)
			: base(contentFilename)
		{
			XmlData = new XmlFile("Content/" + contentFilename + ".xml").Root;
		}
	}
}