using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class PlaneTests
	{
		[Test]
		public void CreatePlane()
		{
			var plane = new Plane(Vector.UnitY, 0.0f);
			Assert.AreEqual(Vector.UnitY, plane.Normal);
			Assert.AreEqual(0.0f, plane.Distance);
		}

		[Test]
		public void RayPlaneIntersect()
		{
			var ray = new Ray(Vector.UnitZ, -Vector.UnitZ);
			var plane = new Plane(Vector.UnitZ, -3.0f);
			Assert.AreEqual(Vector.UnitZ * 3.0f, plane.Intersect(ray));
		}
	}
}