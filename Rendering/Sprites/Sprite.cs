using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// 2D sprite to be rendered, which is an image used as an Entity2D.
	/// </summary>
	public class Sprite : Entity2D
	{
		private Sprite()
			: base(Rectangle.Zero, Color.White) {}

		public Sprite(Image image, Rectangle drawArea)
			: this(image, drawArea, Color.White) {}

		public Sprite(Image image, Rectangle drawArea, Color color)
			: base(drawArea, color)
		{
			if (image == null)
				throw new NullReferenceException("image");
			Add<RenderSprite>();
			Add(image);
		}

		public Image Image
		{
			get { return Get<Image>(); }
			set { Set(value); }
		}
	}
}