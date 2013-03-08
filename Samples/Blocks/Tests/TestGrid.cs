using DeltaEngine.Core;
using DeltaEngine.Rendering;

namespace Blocks.Tests
{
	/// <summary>
	/// Helps test Grid by exposing its list of affixed bricks
	/// </summary>
	public class TestGrid : Grid
	{
		public TestGrid(Renderer renderer, BlocksContent content, Randomizer random)
			: base(renderer, content, random) { }

		public Brick[,] Bricks
		{
			get { return bricks; }
		}

		public int BrickCount
		{
			get
			{
				int count = 0;
				foreach (var brick in Bricks)
					if (brick != null)
						count++;

				return count;
			}
		}
	}
}