using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class RayTests
	{
		[Test]
		public void CreateRay()
		{
			var ray = new Ray(Vector.Zero, Vector.UnitZ);
			Assert.AreEqual(ray.Origin, Vector.Zero);
			Assert.AreEqual(ray.Direction, Vector.UnitZ);
		}
	}
}