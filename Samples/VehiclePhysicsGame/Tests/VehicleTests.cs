using DeltaEngine.Physics2D;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace VehiclePhysicsGame.Tests
{
	internal class VehicleTests : TestWithAllFrameworks
	{
		[Test]
		public void CreateVehicle()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, Physics physics) =>
			{
				var vehicle = new Vehicle(Point.Half, physics, contentLoader);
			});
		}
	}
}