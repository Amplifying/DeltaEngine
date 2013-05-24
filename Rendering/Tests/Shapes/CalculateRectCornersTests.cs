using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class CalculateRectCornersTests : TestWithAllFrameworks
	{
		[Test]
		public void CornersMatchDrawAreaCorners()
		{
			Start(typeof(MockResolver), () =>
			{
				Entity2D entity = CreateEntity();
				var corners = entity.Get<List<Point>>();
				Assert.AreEqual(4, corners.Count);
				var drawArea = entity.Get<Rectangle>();
				Assert.AreEqual(corners[0], drawArea.TopLeft);
				Assert.AreEqual(corners[1], drawArea.TopRight);
				Assert.AreEqual(corners[2], drawArea.BottomRight);
				Assert.AreEqual(corners[3], drawArea.BottomLeft);
			});
		}

		private static Entity2D CreateEntity()
		{
			var entity = new Entity2D(Rectangle.One, Color.White);
			var points = new List<Point>();
			entity.Add(points).Add<Rect.UpdateCorners>();
			EntitySystem.Current.Run();
			return entity;
		}

		[Test]
		public void CornersAreCorrectAfterChangingDrawArea()
		{
			Start(typeof(MockResolver), () =>
			{
				Entity2D entity = CreateEntity();
				var corners = entity.Get<List<Point>>();
				entity.Center = Point.One;
				entity.Size = new Size(1, 2);
				EntitySystem.Current.Run();
				var drawArea = entity.Get<Rectangle>();
				Assert.AreEqual(corners[0], drawArea.TopLeft);
				Assert.AreEqual(corners[1], drawArea.TopRight);
				Assert.AreEqual(corners[2], drawArea.BottomRight);
				Assert.AreEqual(corners[3], drawArea.BottomLeft);
			});
		}
	}
}