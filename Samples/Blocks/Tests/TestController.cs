namespace Blocks.Tests
{
	public class TestController : Controller
	{
		/// <summary>
		/// Helps test Controller by allowing FallingBlock and UpcomingBlock to be assigned
		/// </summary>
		public TestController(Grid grid, SoundManager soundManager)
			: base(grid, soundManager) {}

		public void SetFallingBlock(Block block)
		{
			FallingBlock = block;
		}

		public void SetUpcomingBlock(Block block)
		{
			UpcomingBlock = block;
		}

		public SoundManager SoundManager
		{
			get { return soundManager; }
		}
	}
}