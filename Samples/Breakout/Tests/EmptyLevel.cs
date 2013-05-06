using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace Breakout.Tests
{
	public class EmptyLevel : Level
	{
		public EmptyLevel(EntitySystem entitySystem, ContentLoader content, Score score)
			: base(entitySystem, content, score)
		{
			for (int x = 0; x < rows; x++)
				for (int y = 0; y < columns; y++)
					bricks[x, y].Visibility = Visibility.Hide;
		}
	}
}