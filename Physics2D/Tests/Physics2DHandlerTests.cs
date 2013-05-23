using System;
using DeltaEngine.Entities;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Physics2D.Tests
{
	class Physics2DHandlerTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void FallingWhitheCircle(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, Physics physics, ScreenSpace screen) =>
			{
				CreateFloor(physics, screen);
				var circle = new Ellipse(Point.Half, 0.02f, 0.02f);
				var physicsbody = physics.CreateCircle(0.02f);
				physicsbody.Position = screen.ToPixelSpace(Point.Half);
				physicsbody.Restitution = 0.9f;
				physicsbody.Friction = 0.9f;
				circle.Add(physicsbody);
				entitySystem.Add(circle.Add<RenderPolygon, Physics2D>());
			});
		}

		private static void CreateFloor(Physics physics, ScreenSpace screen)
		{
			physics.CreateEdge(new[] { screen.ToPixelSpace(new Point(0.0f, 0.75f)),
				screen.ToPixelSpace(new Point(1.0f, 0.75f)) }).IsStatic = true;
		}
	}
}
