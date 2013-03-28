using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace Blocks.Tests
{
	public class TestController : Controller
	{
		/// <summary>
		/// Helps test Controller by allowing FallingBlock and UpcomingBlock to be assigned.
		/// Also exposes the Soundbank and whether the falling block is falling fast or not.
		/// </summary>
		public TestController(Grid grid, Soundbank soundbank, BlocksContent content)
			: base(grid, soundbank, content) {}

		public void AssignFixedBlocks()
		{
			FallingBlock = new Block(content, new FixedRandom(), Point.Zero);
			UpcomingBlock = new Block(content, new FixedRandom(), Point.Zero);
		}

		public void SetFallingBlock(Block block)
		{
			FallingBlock = block;
		}

		public void SetUpcomingBlock(Block block)
		{
			UpcomingBlock = block;
		}

		public Soundbank Soundbank
		{
			get { return soundbank; }
		}
	}
}