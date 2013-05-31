using System;
using DeltaEngine.Core;

namespace GameOfDeath
{
	/// <summary>
	/// Good old Game of Life, which will be the basis of multiplying rabbits in this game
	/// </summary>
	public class GameOfLife : Runner
	{
		public GameOfLife(int width, int height)
		{
			if (width <= 0 || height <= 0)
				throw new SizeMustBeGreaterThanZero();

			this.width = width;
			this.height = height;
			currentWorld = new bool[width,height];
			nextGeneration = new bool[width,height];
		}

		public class SizeMustBeGreaterThanZero : Exception {}

		public readonly int width;
		public readonly int height;
		private readonly bool[,] currentWorld;
		private readonly bool[,] nextGeneration;

		public bool this[int x, int y]
		{
			get { return currentWorld[x, y]; }
			set { currentWorld[x, y] = value; }
		}

		public virtual void Run()
		{
			EvolveCurrentWorldIntoNextGeneration();
			CopyNextGenerationIntoCurrentWorld();
			GenerationCount++;
		}

		public int GenerationCount { get; private set; }

		private void EvolveCurrentWorldIntoNextGeneration()
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					nextGeneration[x, y] = ShouldSurvive(x, y);
		}

		public virtual bool ShouldSurvive(int x, int y)
		{
			bool isAlive = this[x, y];
			int neighbours = GetNumberOfNeighbours(x, y);
			return ActiveCellShouldHaveTwoOrThreeNeighbors(isAlive, neighbours) ||
				ResurrectDeadCellWithExactlyThreeNeighbors(isAlive, neighbours);
		}

		private static bool ActiveCellShouldHaveTwoOrThreeNeighbors(bool isAlive, int neighbours)
		{
			return isAlive && (neighbours == 2 || neighbours == 3);
		}

		private static bool ResurrectDeadCellWithExactlyThreeNeighbors(bool isAlive, int neighbours)
		{
			return !isAlive && neighbours == 3;
		}

		protected int GetNumberOfNeighbours(int x, int y)
		{
			int neigbours = 0;
			for (int xIndex = x - 1; xIndex <= x + 1; xIndex++)
				for (int yIndex = y - 1; yIndex <= y + 1; yIndex++)
					if ((xIndex != x || yIndex != y) && IsNeighborSet(xIndex, yIndex))
						neigbours++;

			return neigbours;
		}

		private bool IsNeighborSet(int xIndex, int yIndex)
		{
			return ValidPosition(xIndex, yIndex) && this[xIndex, yIndex];
		}

		private bool ValidPosition(int xIndex, int yIndex)
		{
			return xIndex >= 0 && yIndex >= 0 && xIndex < width && yIndex < height;
		}

		private void CopyNextGenerationIntoCurrentWorld()
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					currentWorld[x, y] = nextGeneration[x, y];
		}

		public void Randomize()
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					currentWorld[x, y] = Randomizer.Current.Get(0, 3) == 1;
		}
	}
}