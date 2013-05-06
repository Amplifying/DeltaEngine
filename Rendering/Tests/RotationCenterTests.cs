using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class RotationCenterTests
	{
		[Test]
		public void AssignAndChangeRotationCenter()
		{
			var center = new RotationCenter(Point.Zero);
			Assert.AreEqual(Point.Zero, center.Value);
			center.Value = Point.One;
			Assert.AreEqual(Point.One, center.Value);
		}
	}
}