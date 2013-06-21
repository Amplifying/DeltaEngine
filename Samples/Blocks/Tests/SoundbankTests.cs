using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Soundbank class
	/// </summary>
	public class SoundbankTests : TestWithAllFrameworks
	{
		[Test]
		public void Constructor()
		{
			Start(typeof(MockResolver), (JewelBlocksContent content, Soundbank soundbank) =>
			{
				Assert.IsNotNull(soundbank.BlockAffixed);
				Assert.IsNotNull(soundbank.BlockCouldntMove);
				Assert.IsNotNull(soundbank.BlockMoved);
				Assert.IsNotNull(soundbank.GameLost);
				Assert.IsNotNull(soundbank.MultipleRowsRemoved);
				Assert.IsNotNull(soundbank.RowRemoved);
			});
		}
	}
}