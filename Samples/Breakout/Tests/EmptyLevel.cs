using DeltaEngine.Core;
using DeltaEngine.Rendering;

namespace Breakout.Tests
{
	public class EmptyLevel : Level
	{
		public EmptyLevel(Content content, Renderer renderer, Score score)
			: base(content, renderer, score)
		{
			for (int x = 0; x < rows; x++)
				for (int y = 0; y < columns; y++)
					bricks[x, y].Dispose();
		}

		public override Sprite GetBrickAt(float x, float y)
		{
			return null;
		}
	}
}