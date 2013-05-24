using DeltaEngine.Content;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit test for JewelBlocksContent
	/// </summary>
	public class JewelBlocksContentTests : TestWithAllFrameworks
	{
		[Test]
		public void Constructor()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				var content = new JewelBlocksContent(contentLoader);
				Assert.AreEqual("JewelBlocks_", content.Prefix);
				Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
				Assert.IsFalse(content.DoBricksSplitInHalfWhenRowFull);
			});
		}
	}
}