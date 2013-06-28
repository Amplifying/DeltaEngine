using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja
{
	public class LogoLink : Sprite
	{
		public LogoLink(Image image, string url, float size) : base(image, new Rectangle(0, 0, size, size))
		{
			this.Url = url;
			reduceSize = new Size(size * 0.18f);
		}

		public string Url { get; private set; }
		private Size reduceSize;
		
		public bool IsHover(Point point)
		{
			var hoverZone = DrawArea.Reduce(reduceSize);
			return hoverZone.Contains(point);
		}
	}
}
