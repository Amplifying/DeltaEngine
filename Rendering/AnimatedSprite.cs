using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// AnimatedSprites are 2D quads rendered automatically on the specified location
	/// changing the main image for the next one in a determined fps, etc.
	/// </summary>
	public class AnimatedSprite : Sprite
	{
		public AnimatedSprite(Image image, Rectangle initialDrawArea, Color color, int width, 
			int height, int spritesPerSecond, Point[] frames)
			: base(image, initialDrawArea, color)
		{
			sizeSprite.Width = width;
			sizeSprite.Height = height;
			spriteTime = (1.0f / spritesPerSecond);
			this.frames = frames;
		}

		private readonly Size sizeSprite;
		private readonly Point[] frames;
		private readonly float spriteTime;

		protected override void Render(Renderer renderer, Time time)
		{
			screen = renderer.Screen;
			DrawImage(time);
		}

		private void DrawImage(Time time)
		{
			var sprite = GetCurrentSprite(time);
			var heightSprite = 1.0f / Image.PixelSize.Height * sizeSprite.Height;
			var widthSprite = 1.0f / Image.PixelSize.Width * sizeSprite.Width;
			CalculateCornersAndPaint(sprite, widthSprite, heightSprite);
		}

		private Point GetCurrentSprite(Time time)
		{
			frameOffset += time.CurrentDelta;
			if (frameOffset > spriteTime)
			{
				frameNumber = (frameNumber + 1) % frames.Length;
				frameOffset = 0;
			}
			return frames[frameNumber];
		}

		private int frameNumber;
		private float frameOffset;

		private void CalculateCornersAndPaint(Point sprite, float widthSprite, float heightSprite)
		{
			var topLeft = new Point(sprite.X * widthSprite, sprite.Y * heightSprite);
			var topRight = new Point((sprite.X + 1.0f) * widthSprite, sprite.Y * heightSprite);
			var botLeft = new Point(sprite.X * widthSprite, (sprite.Y + 1.0f) * heightSprite);
			var botRight = new Point((sprite.X + 1.0f) * widthSprite, (sprite.Y + 1.0f) * heightSprite);

			var vertices = new[]
			{
				GetVertex(Rotate(DrawArea.TopLeft), topLeft), 
				GetVertex(Rotate(DrawArea.TopRight), topRight),
				GetVertex(Rotate(DrawArea.BottomRight), botRight),
				GetVertex(Rotate(DrawArea.BottomLeft), botLeft)
			};
			Image.Draw(vertices);
		}
	}
}
