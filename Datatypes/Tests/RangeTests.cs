using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	class RangeTests
	{
		[Test]
		public void CreateRange()
		{
			var range = new Range(1.2f, 2.5f);
			Assert.AreEqual(1.2f, range.Minimum);
			Assert.AreEqual(2.5f, range.Maximum);
			Assert.LessOrEqual(1.2f, range.GetRandomValue());
			Assert.GreaterOrEqual(2.5f, range.GetRandomValue());
		}

		[Test]
		public void GetRandomValue()
		{
			var range = new Range(1.2f, 2.5f);
			Assert.LessOrEqual(1.2f, range.GetRandomValue());
			Assert.GreaterOrEqual(2.5f, range.GetRandomValue());
		}
	}
}
