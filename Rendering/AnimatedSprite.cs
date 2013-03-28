using System.Collections.Generic;
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
		public AnimatedSprite(IList<Image> images, Rectangle initialDrawArea)
			: base(images[0], initialDrawArea)
		{
			spriteTime = 0.25f;
			this.images = images;
			currentFrame = 0;
		}

		private float spriteTime;
		private readonly IList<Image> images;
		private int currentFrame;

		public void AddImage(Image image)
		{
			images.Add(image);
		}

		public void SetNumberSpritesPerSecond(int number)
		{
			spriteTime = 1.0f / number;
		}

		protected override void Render(Renderer renderer, Time time)
		{
			screen = renderer.Screen;
			DrawImage(time);
		}

		private void DrawImage(Time time)
		{
			CalculateCurrentFrame(time);
			CalculateCornersAndPaint();
		}

		private void CalculateCurrentFrame(Time time)
		{
			frameOffset += time.CurrentDelta;
			if (frameOffset > spriteTime)
			{
				currentFrame = (currentFrame + 1) % images.Count;
				frameOffset = 0;
			}
		}

		private float frameOffset;

		private void CalculateCornersAndPaint()
		{
			var vertices = new[]
			{
				GetVertex(Rotate(DrawArea.TopLeft), Point.Zero), 
				GetVertex(Rotate(DrawArea.TopRight), Point.UnitX),
				GetVertex(Rotate(DrawArea.BottomRight), Point.One),
				GetVertex(Rotate(DrawArea.BottomLeft), Point.UnitY)
			};
			images[currentFrame].Draw(vertices);
		}
	}
}