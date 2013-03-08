using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Soundbank class
	/// </summary>
	public class SoundbankTests
	{
		[Test]
		public void Constructor()
		{
			var soundbank = new TestResolver().Resolve<Soundbank>();
			Assert.IsNotNull(soundbank.BlockAffixed);
			Assert.IsNotNull(soundbank.BlockCouldntMove);
			Assert.IsNotNull(soundbank.BlockMoved);
			Assert.IsNotNull(soundbank.GameLost);
			Assert.IsNotNull(soundbank.MultipleRowsRemoved);
			Assert.IsNotNull(soundbank.RowRemoved);
		}
	}
}
