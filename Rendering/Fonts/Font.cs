using DeltaEngine.Content;
using DeltaEngine.Core.Xml;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Fonts
{
	public class Font
	{
		public Font(ContentLoader content, string fontContentName)
		{
			Data = new FontData(content.Load<XmlContent>(fontContentName).Data);
			Image = content.Load<Image>(Data.FontMapName);
		}

		public FontData Data { get; private set; }
		public Image Image { get; private set; }
	}
}