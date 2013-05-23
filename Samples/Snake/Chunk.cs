using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace Snake
{
	public class Chunk : Rect
	{
		public Chunk(int gridSize, float blockSize)
			: base(new Rectangle(new Point(blockSize * (gridSize / 2), blockSize * (gridSize / 2)),
				new Size(blockSize)), Color.Purple)
		{
			this.gridSize = gridSize;
			this.blockSize = blockSize;
			random = new PseudoRandom();
		}

		private readonly int gridSize;
		private readonly float blockSize;
		private readonly PseudoRandom random;

		public void SpawnAtRandomLocation()
		{
			int x = random.Get(2, gridSize - 2);
			int y = random.Get(2, gridSize - 2);
			var newRandomPos = new Point(x * blockSize, y * blockSize);
			var newDrawArea = new Rectangle(newRandomPos, new Size(blockSize));
			DrawArea = newDrawArea;
		}

		public bool IsCollidingWithSnake(List<Rect> snakeBodies)
		{
			return snakeBodies.Any(IsBodyColliding);
		}

		private bool IsBodyColliding(Entity2D body)
		{
			return body.DrawArea.IsColliding(DrawArea.Reduce(new Size(0.01f)));
		}
	}
}