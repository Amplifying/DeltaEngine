using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit test for FruitBlocksContent
	/// </summary>
	public class FruitBlocksContentTests
	{
		[Test]
		public void Constructor()
		{
			var content = new FruitBlocksContent(new TestResolver());
			Assert.AreEqual("FruitBlocks_", content.Prefix);
			Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
			Assert.IsTrue(content.DoBricksSplitInHalfWhenRowFull);
		}
	}
}