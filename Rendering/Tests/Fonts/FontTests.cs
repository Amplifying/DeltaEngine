using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	internal class FontTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawSmallFont(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var font = new Font(content, "Verdana12");
				new FontText(font, "Hi there", Point.Half);
			});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawBigFont(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var font = new Font(content, "Tahoma30");
				new FontText(font, "Big Fonts rule!", Point.Half);
			});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawColoredFonts(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var font = new Font(content, "Tahoma30");
				new FontText(font, "Red Text", new Point(0.5f, 0.4f)) { Color = Color.Red };
				new FontText(font, "Yellow", new Point(0.5f, 0.6f)) { Color = Color.Yellow };
			});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawFontAndLines(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new Line2D(Point.Zero, Point.One, Color.Red);
				new FontText(new Font(content, "Tahoma30"), "Delta Engine", new Point(0.5f, 0.4f));
				new Line2D(Point.UnitX, Point.UnitY, Color.Red);
			});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void DrawFontAndSprite(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.PaleGreen);
				new FontText(new Font(content, "Tahoma30"), "Delta Engine", new Point(0.5f, 0.4f));
			});
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);

		[VisualTest, ApproveFirstFrameScreenshot]
		public void Draw2DifferentFonts(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new FontText(new Font(content, "Tahoma30"), "Delta Engine", new Point(0.5f, 0.4f));
				new FontText(new Font(content, "Verdana12"), "Delta Engine", new Point(0.5f, 0.6f));
			});
		}
	}
}