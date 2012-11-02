using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.OpenTK.Tests
{
	public class DrawingTests
	{
		[Test, Ignore]
		public void DrawColoredRectangle()
		{
			Drawing rememberDrawing = null;
			App.Start((Drawing drawing) => rememberDrawing = drawing, delegate
			{
				rememberDrawing.SetColor(Color.Red);
				rememberDrawing.DrawRectangle(Rectangle.One);
			});
		}
	}
}