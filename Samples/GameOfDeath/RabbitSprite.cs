using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace GameOfDeath
{
	/// <summary>
	/// Like BouncingLogo rabbits are just sprites bouncing around in a limited area box.
	/// </summary>
	public class RabbitSprite : Sprite
	{
		public RabbitSprite(Image image, Point position)
			: base(image, Rectangle.One)
		{
			DrawArea = Rectangle.FromCenter(position, OriginalSize);
			BoundingBox = Rectangle.FromCenter(position, OriginalSize * 1.2f);
			Visibility = Visibility.Hide;
			RenderLayer = (int)GameCoordinator.RenderLayers.Rabbits;
			Add(new SimplePhysics.Data
			{
				Velocity =
					new Point(Randomizer.Current.Get(-0.035f, 0.035f), Randomizer.Current.Get(-0.025f, 0.025f))
			});
		}

		public Rectangle BoundingBox { get; private set; }

		public Size OriginalSize
		{
			get { return Image.PixelSize / Scoreboard.QuadraticFullscreenSize; }
		}

		public Point Velocity
		{
			get { return Get<SimplePhysics.Data>().Velocity; }
			set { Get<SimplePhysics.Data>().Velocity = value; }
		}
	}
}