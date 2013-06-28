using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Sprites
{
	public class SpriteTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderSprite()
		{
			new Sprite("DeltaEngineLogo", screenCenter);
		}

		[Test]
		public void RenderWalkingCharacterSprite()
		{
			new Sprite("DeltaEngineLogo", screenCenter);
			new Sprite("WalkingCharacter", screenCenter);
		}

		[Test]
		public void RenderRedSpriteOverBlue()
		{
			new Sprite("DeltaEngineLogo", screenCenter) { Color = Color.Red, RenderLayer = 1 };
			new Sprite("DeltaEngineLogo", screenTopLeft) { Color = Color.Blue, RenderLayer = 0 };
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);
		private readonly Rectangle screenTopLeft = Rectangle.FromCenter(0.3f, 0.3f, 0.5f, 0.5f);

		[Test]
		public void CreateSprite()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			Assert.AreEqual(Color.White, sprite.Color);
			Assert.AreEqual("DeltaEngineLogo", sprite.Image.Name);
			Assert.AreEqual(new Size(128, 128), sprite.Image.PixelSize);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeImage()
		{
			var sprite = new Sprite("DeltaEngineLogo", screenCenter);
			Assert.AreEqual("DeltaEngineLogo", sprite.Image.Name);
			Assert.AreEqual(BlendMode.Normal, sprite.Get<BlendMode>());
			sprite.Image = ContentLoader.Load<Image>("Verdana12Font");
			Assert.AreEqual("Verdana12Font", sprite.Image.Name);
			Assert.AreEqual(BlendMode.Normal, sprite.Get<BlendMode>());
			Window.CloseAfterFrame();
		}

		[Test]
		public void RenderSpriteAndLines()
		{
			new Line2D(Point.Zero, Point.One, Color.Blue);
			new Sprite("DeltaEngineLogo", screenCenter);
			new Line2D(Point.UnitX, Point.UnitY, Color.Purple);
		}

		[Test]
		public void DrawingTwoSpritesWithTheSameImageAndRenderLayerOnlyIssuesOneDrawCall()
		{
			new Sprite("DeltaEngineLogo", screenCenter);
			new Sprite("DeltaEngineLogo", screenCenter);
			RunCode = () => Assert.AreEqual(1, Resolve<Drawing>().NumberOfTimesDrawn);
			Window.CloseAfterFrame();
		}

		[Test]
		public void DrawingTwoSpritesWithTheSameImageButDifferentRenderLayersIssuesTwoDrawCalls()
		{
			new Sprite("DeltaEngineLogo", screenCenter) { RenderLayer = 1 };
			new Sprite("DeltaEngineLogo", screenCenter) { RenderLayer = 2 };
			RunCode = () => Assert.AreEqual(2, Resolve<Drawing>().NumberOfTimesDrawn);
			Window.CloseAfterFrame();
		}

		[Test]
		public void DrawingTwoSpritesWithDifferentImagesIssuesTwoDrawCalls()
		{
			new Sprite("DeltaEngineLogo", screenCenter);
			new Sprite("WalkingCharacter", screenCenter);
			RunCode = () => Assert.AreEqual(2, Resolve<Drawing>().NumberOfTimesDrawn);
			Window.CloseAfterFrame();
		}

		[Test]
		public void DrawSpritesWithDifferentBlendModes()
		{
			Window.Title = "Blend modes: Opaque, Normal, Additive; AlphaTest, LightEffect, Subtractive";
			var drawAreas = CreateDrawAreas(3, 2);
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var alpha = ContentLoader.Load<Image>("DeltaEngineLogoAlpha");
			new Sprite(image, drawAreas[0]) { BlendMode = BlendMode.Opaque };
			new Sprite(alpha, drawAreas[1]) { BlendMode = BlendMode.Opaque };
			new Sprite(image, drawAreas[2]) { BlendMode = BlendMode.Normal };
			new Sprite(alpha, drawAreas[3]) { BlendMode = BlendMode.Normal };
			new Sprite(image, drawAreas[4]) { BlendMode = BlendMode.Additive };
			new Sprite(alpha, drawAreas[5]) { BlendMode = BlendMode.Additive };
			new Sprite(image, drawAreas[6]) { BlendMode = BlendMode.AlphaTest };
			new Sprite(alpha, drawAreas[7]) { BlendMode = BlendMode.AlphaTest };
			new Sprite(image, drawAreas[8]) { BlendMode = BlendMode.LightEffect };
			new Sprite(alpha, drawAreas[9]) { BlendMode = BlendMode.LightEffect };
			new Sprite(image, drawAreas[10]) { BlendMode = BlendMode.Subtractive };
			new Sprite(alpha, drawAreas[11]) { BlendMode = BlendMode.Subtractive };
		}

		private static Rectangle[] CreateDrawAreas(int cols, int rows)
		{
			var drawAreas = new Rectangle[cols * rows * 2];
			var size = new Size(0.2f, 0.2f);
			var position1 = new Point(0.2f, 0.35f);
			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < cols; x++)
				{
					var index = x * 2 + (y * cols * rows);
					drawAreas[index] = Rectangle.FromCenter(position1, size);
					var position2 = new Point(position1.X + 0.04f, position1.Y + 0.04f);
					drawAreas[index + 1] = Rectangle.FromCenter(position2, size);
					position1.X += 0.3f;
				}
				position1 = new Point(0.2f, position1.Y + 0.275f);
			}
			return drawAreas;
		}

		[Test]
		public void DrawSpritesWithBlendModeFromContentMetaData()
		{
			var drawAreas = CreateDrawAreas(3, 1);
			new Sprite("DeltaEngineLogo", drawAreas[0]);
			new Sprite("DeltaEngineLogoOpaque", drawAreas[1]);
			new Sprite("DeltaEngineLogo", drawAreas[2]);
			new Sprite("DeltaEngineLogoAlpha", drawAreas[3]);
			new Sprite("DeltaEngineLogo", drawAreas[4]);
			new Sprite("DeltaEngineLogoAdditive", drawAreas[5]);
		}
	}
}