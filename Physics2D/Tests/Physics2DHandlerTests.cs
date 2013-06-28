using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	class Physics2DHandlerTests : TestWithMocksOrVisually
	{
		[Test]
		public void FallingWhiteCircle()
		{
			CreateFloor(Resolve<Physics>(), Resolve<ScreenSpace>());
				var circle = new Ellipse(Point.Half, 0.02f, 0.02f, Color.White);
				var physicsbody = Resolve<Physics>().CreateCircle(0.02f);
				physicsbody.Position = Resolve<ScreenSpace>().ToPixelSpace(Point.Half);
				physicsbody.Restitution = 0.9f;
				physicsbody.Friction = 0.9f;
				circle.Add(physicsbody);
				circle.Start<Polygon.Render, Physics2D>();
		}

		private static void CreateFloor(Physics physics, ScreenSpace screen)
		{
			physics.CreateEdge(new[] { screen.ToPixelSpace(new Point(0.0f, 0.75f)),
				screen.ToPixelSpace(new Point(1.0f, 0.75f)) }).IsStatic = true;
		}
	}
}
