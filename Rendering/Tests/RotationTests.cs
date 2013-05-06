using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class RotationTests
	{
		[Test]
		public void NewRotation()
		{
			var rotation = new Rotation();
			Assert.AreEqual(0.0f, rotation.Value);
		}

		[Test]
		public void ChangeRotation()
		{
			var rotation = new Rotation(5.0f);
			Assert.AreEqual(5.0f, rotation.Value);
			rotation.Value = 10.0f;
			Assert.AreEqual(10.0f, rotation.Value);
		}
	}
}