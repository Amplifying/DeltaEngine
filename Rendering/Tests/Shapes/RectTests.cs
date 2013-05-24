using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RectTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderRect(Type resolver)
		{
			Start(resolver, () => new Rect(new Rectangle(0.3f, 0.3f, 0.4f, 0.4f), Color.Blue));
		}

		[Test]
		public void DefaultRectIsRectangleZeroAndWhite()
		{
			var rect = new Rect();
			Assert.AreEqual(Rectangle.Zero, rect.DrawArea);
			Assert.AreEqual(Color.White, rect.Color);
		}

		[IntegrationTest]
		public void RectangleCornersAreCorrect(Type resolver)
		{
			Start(resolver, () =>
			{
				var rect = new Rect(Rectangle.One, Color.White);
				EntitySystem.Current.Run();
				var corners = new List<Point> { Point.Zero, Point.UnitX, Point.One, Point.UnitY };
				Assert.AreEqual(corners, rect.Points);
			});
		}
	}
}