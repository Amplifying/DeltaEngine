using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	internal class FontTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawSmallFont()
		{
			new FontText(new Font("Verdana12"), "Hi there", Point.Half);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawBigFont()
		{
			new FontText(new Font("Tahoma30"), "Big Fonts rule!", Point.Half);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawColoredFonts()
		{
			var font = new Font("Tahoma30");
			new FontText(font, "Red Text", new Point(0.5f, 0.4f)) { Color = Color.Red };
			new FontText(font, "Yellow", new Point(0.5f, 0.6f)) { Color = Color.Yellow };
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawFontAndLines()
		{
			new Line2D(Point.Zero, Point.One, Color.Red);
			new FontText(new Font("Tahoma30"), "Delta Engine", new Point(0.5f, 0.4f));
			new Line2D(Point.UnitX, Point.UnitY, Color.Red);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawFontAndSprite()
		{
			new Sprite("DeltaEngineLogo", screenCenter) { Color = Color.PaleGreen };
			new FontText(new Font("Tahoma30"), "Delta Engine", new Point(0.5f, 0.4f));
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);

		[Test, ApproveFirstFrameScreenshot]
		public void Draw2DifferentFonts()
		{
			new FontText(new Font("Tahoma30"), "Delta Engine", new Point(0.5f, 0.4f));
			new FontText(new Font("Verdana12"), "Delta Engine", new Point(0.5f, 0.6f));
		}
	}
}