using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	internal class AffixToPhysicsTests : TestWithMocksOrVisually
	{
		[Test]
		public void FallingWhiteCircle()
		{
			Resolve<Window>();
			CreateFloor(Resolve<Physics>());
			var circle = new Ellipse(Point.Half, 0.02f, 0.02f, Color.White);
			var physicsbody = Resolve<Physics>().CreateCircle(0.02f);
			physicsbody.Position = ScreenSpace.Current.ToPixelSpace(Point.Half);
			physicsbody.Restitution = 0.9f;
			physicsbody.Friction = 0.9f;
			circle.Add(physicsbody);
			circle.Start<AffixToPhysics>();
		}

		private static void CreateFloor(Physics physics)
		{
			physics.CreateEdge(new[]
			{
				ScreenSpace.Current.ToPixelSpace(new Point(0.0f, 0.75f)),
				ScreenSpace.Current.ToPixelSpace(new Point(1.0f, 0.75f))
			}).IsStatic = true;
		}
	}
}