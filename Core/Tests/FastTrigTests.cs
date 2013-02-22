using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class FastTrigTests
	{
		[Test]
		public void SinRightAngles()
		{
			var trig = new FastTrig();
			Assert.AreEqual(0.0f, trig.Sin(0.0f));
			Assert.AreEqual(1.0f, trig.Sin(90.0f));
			Assert.AreEqual(0.0f, trig.Sin(180.0f));
			Assert.AreEqual(-1.0f, trig.Sin(270.0f));
		}

		[Test]
		public void CosRightAngles()
		{
			var trig = new FastTrig();
			Assert.AreEqual(1.0f, trig.Cos(0.0f));
			Assert.AreEqual(0.0f, trig.Cos(90.0f));
			Assert.AreEqual(-1.0f, trig.Cos(180.0f));
			Assert.AreEqual(0.0f, trig.Cos(270.0f));
		}

		[Test]
		public void Sin()
		{
			var trig = new FastTrig();
			Assert.AreEqual(MathExtensions.Sin(55), trig.Sin(55), 0.00001f);
			Assert.AreEqual(MathExtensions.Sin(76.7f), trig.Sin(76.7f), 0.00001f);
			Assert.AreEqual(MathExtensions.Sin(111.7f), trig.Sin(111.7f), 0.00001f);
			Assert.AreEqual(MathExtensions.Sin(191.1f), trig.Sin(191.1f), 0.00001f);
			Assert.AreEqual(MathExtensions.Sin(295.2f), trig.Sin(295.2f), 0.00001f);
			Assert.AreEqual(MathExtensions.Sin(380), trig.Sin(380), 0.00001f);
			Assert.AreEqual(MathExtensions.Sin(-133), trig.Sin(-133), 0.00001f);
		}

		[Test]
		public void Cos()
		{
			var trig = new FastTrig();
			Assert.AreEqual(MathExtensions.Cos(35), trig.Cos(35), 0.00001f);
			Assert.AreEqual(MathExtensions.Cos(71.7f), trig.Cos(71.7f), 0.00001f);
			Assert.AreEqual(MathExtensions.Cos(133.7f), trig.Cos(133.7f), 0.00001f);
			Assert.AreEqual(MathExtensions.Cos(201.2f), trig.Cos(201.2f), 0.00001f);
			Assert.AreEqual(MathExtensions.Cos(285.2f), trig.Cos(285.2f), 0.00001f);
			Assert.AreEqual(MathExtensions.Cos(801), trig.Cos(801), 0.00001f);
			Assert.AreEqual(MathExtensions.Cos(-1234), trig.Cos(-1234), 0.00001f);
		}
	}
}