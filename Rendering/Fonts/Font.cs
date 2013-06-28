using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Fonts
{
	public class Font
	{
		public Font(string fontContentName)
		{
			Data = new FontData(ContentLoader.Load<XmlContent>(fontContentName).Data);
			Image = ContentLoader.Load<Image>(Data.FontMapName);
		}

		public FontData Data { get; private set; }
		public Image Image { get; private set; }
	}
}