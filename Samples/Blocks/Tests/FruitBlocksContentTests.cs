using DeltaEngine.Content;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit test for FruitBlocksContent
	/// </summary>
	public class FruitBlocksContentTests : TestWithAllFrameworks
	{
		[Test]
		public void Constructor()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				var content = new FruitBlocksContent(contentLoader);
				Assert.AreEqual("FruitBlocks_", content.Prefix);
				Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
				Assert.IsTrue(content.DoBricksSplitInHalfWhenRowFull);
			});
		}
	}
}