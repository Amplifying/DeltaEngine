using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace Dark
{
	public class LevelObject : Sprite
	{
		public LevelObject(Image image, Vector position, Size size, float rotation)
			: base(image, Rectangle.Zero)
		{
			Position = position;
			Rotation = rotation;
			Center = Point.Zero;
			DrawArea = Rectangle.FromCenter(position.X, position.Y, size.Width, size.Height);
			RenderLayer = 0;
			//Add<RenderShadow>();
		}

		public Vector Position { get; private set; }
	}
}