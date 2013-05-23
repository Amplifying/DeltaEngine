using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class CalculateRectCornersTests
	{
		[SetUp]
		public void CreateEntity()
		{
			entitySystem = new MockResolver().EntitySystem;
			entity = new Entity2D(Rectangle.One, Color.White);
			var points = new List<Point>();
			entity.Add(points).Add<CalculateRectCorners>();
			entitySystem.Add(entity);
			entitySystem.Run();
		}

		private EntitySystem entitySystem;
		private Entity2D entity;

		[Test]
		public void CornersMatchDrawAreaCorners()
		{
			var corners = entity.Get<List<Point>>();
			Assert.AreEqual(4, corners.Count);
			var drawArea = entity.Get<Rectangle>();
			Assert.AreEqual(corners[0], drawArea.TopLeft);
			Assert.AreEqual(corners[1], drawArea.TopRight);
			Assert.AreEqual(corners[2], drawArea.BottomRight);
			Assert.AreEqual(corners[3], drawArea.BottomLeft);
		}

		[Test]
		public void CornersAreCorrectAfterChangingDrawArea()
		{
			var corners = entity.Get<List<Point>>();
			entity.Center = Point.One;
			entity.Size = new Size(1, 2);
			entitySystem.Run();
			var drawArea = entity.Get<Rectangle>();
			Assert.AreEqual(corners[0], drawArea.TopLeft);
			Assert.AreEqual(corners[1], drawArea.TopRight);
			Assert.AreEqual(corners[2], drawArea.BottomRight);
			Assert.AreEqual(corners[3], drawArea.BottomLeft);
		}
	}
}