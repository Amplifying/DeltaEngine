using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Scenes.UserInterfaces.Terminal;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces.Terminal
{
	public class TextLineTests : TestWithMocksOrVisually
	{
		[Test]
		public void TestTextDrawAreaCalculation()
		{
			var screenSpace = new QuadraticScreenSpace(Window);
			float fontSize = screenSpace.FromPixelSpace(new Size(FontPixelSize)).Height;
			var textLine = new TextLine("Hello World", fontSize);
			var drawArea = textLine.fontText.DrawArea;
			Assert.True(drawArea.Left.IsNearlyEqual(0.1031f));
			Assert.True(drawArea.Top.IsNearlyEqual(0.0188f));
			Assert.True(drawArea.Height.IsNearlyEqual(0.0188f));
			Assert.True(drawArea.Width.IsNearlyEqual(0.0188f));
			Window.CloseAfterFrame();
		}

		private const float FontPixelSize = Scenes.UserInterfaces.Terminal.Terminal.FontPixelSize;

		[Test]
		public void TestSetTextPosition()
		{
			var screenSpace = new QuadraticScreenSpace(Window);
			float fontSize = screenSpace.FromPixelSpace(new Size(FontPixelSize)).Height;
			var textLine = new TextLine("Hello World", fontSize);
			textLine.Position = Point.Half;
			var newDrawArea = textLine.fontText.DrawArea;
			Assert.True(newDrawArea.Left.IsNearlyEqual(0.6031f));
			Assert.True(newDrawArea.Top.IsNearlyEqual(0.5188f));
			Assert.True(newDrawArea.Height.IsNearlyEqual(0.0188f));
			Assert.True(newDrawArea.Width.IsNearlyEqual(0.0188f));
			Window.CloseAfterFrame();
		}
	}
}