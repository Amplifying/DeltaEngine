using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit test for JewelBlocksContent
	/// </summary>
	public class JewelBlocksContentTests
	{
		[Test]
		public void Constructor()
		{
			var content = new JewelBlocksContent(new TestResolver());
			Assert.AreEqual("JewelBlocks_", content.Prefix);
			Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
			Assert.IsFalse(content.DoBricksSplitInHalfWhenRowFull);
		}
	}
}