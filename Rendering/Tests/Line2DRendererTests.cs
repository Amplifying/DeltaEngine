using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	/// <summary>
	/// Automatic tests to see if the Line2D renderable class behaves as it should, no visual stuff
	/// here. See Line2DTests for VisualTests with Line2D instead.
	/// </summary>
	public class Line2DRendererTests : RendererTests
	{
		[SetUp]
		public new void Init()
		{
			resolver.NumberOfVerticesDrawn = 0;
		}

		[Test]
		public void Create()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red);
			renderer.Add(line);
			Assert.AreEqual(Color.Red, line.Color);
			renderer.Remove(line);
		}

		[Test]
		public void Render()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Red);
			renderer.Add(line);
			line.EndPosition = Point.Half;
			renderer.Run(null);
			Assert.AreEqual(2, resolver.NumberOfVerticesDrawn);
			renderer.RemoveAll();
		}
	}
}