using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class CalculateEllipsePointsTests : TestWithAllFrameworks
	{
		[Test]
		public void BorderPointsAreCalculatedOnRunningEntitySystem()
		{
			Start(typeof(MockResolver), () =>
			{
				var entity = new Entity2D(Rectangle.One, Color.White);
				var points = new List<Point>();
				entity.Add(points).Add<Ellipse.UpdatePoints>();
				EntitySystem.Current.Run();
				Assert.AreEqual(48, points.Count);
			});
		}
	}
}