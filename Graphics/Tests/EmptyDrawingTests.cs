using DeltaEngine.Datatypes;
using Moq;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class EmptyDrawingTests : Drawing
	{
		public EmptyDrawingTests()
			: base(new Mock<Device>().Object) {}

		public override void SetColor(Color color) {}

		public override void DrawRectangle(Rectangle area) {}

		[Test]
		public void DrawColoredRectangle()
		{
			Assert.NotNull(GraphicsDevice);
			SetColor(Color.Red);
			DrawRectangle(Rectangle.One);
		}
	}
}