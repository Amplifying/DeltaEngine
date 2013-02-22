using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	class GameOfAiTests : TestStarter
	{
		[VisualTest]
		public void ShowGameOfLife(Type resolver)
		{
			var game = new GameOfAi(24, 24);
			game.Randomize();
			Start(resolver, (Window window) => window.TotalPixelSize = new Size(1024, 1024),
				(Renderer renderer, Time time) =>
				{
					if (resolver == typeof(TestResolver) || time.CheckEvery(0.1f))
						game.Run();

					for (int x = 0; x < game.width; x++)
						for (int y = 0; y < game.height; y++)
						{
							float posX = 0.1f + 0.8f * x / game.width;
							float posY = 0.1f + 0.8f * y / game.height;
							renderer.DrawRectangle(Rectangle.FromCenter(posX, posY, 0.025f, 0.025f),
								game[x, y] ? Color.White : Color.DarkGray);
						}
				});
		}
	}

	/// <summary>
	/// Testing different Ai learning strategies for the Game Of Life to learn the best surviving
	/// strategy by itself.
	/// </summary>
	internal class GameOfAi : GameOfLife
	{
		public GameOfAi(int width, int height)
			: base(width, height)
		{
			aliveCount = new int[width, height];
			foodAvailablePerRound = width * height / 2;
			aliveStrategy = new int[width,height];
			notAliveStrategy = new int[width, height];
			RandomizeStrategies();
		}

		private readonly int[,] aliveCount;
		private readonly int foodAvailablePerRound;
		private readonly int[,] aliveStrategy;
		private readonly int[,] notAliveStrategy;

		private void RandomizeStrategies()
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
				{
					aliveStrategy[x, y] = random.Get(MinNeighbours, MaxNeighbours);
					notAliveStrategy[x, y] = random.Get(MinNeighbours, MaxNeighbours);
				}
		}

		readonly PseudoRandom random = new PseudoRandom();
		private const int MinNeighbours = 0;
		private const int MaxNeighbours = 8;

		public override bool ShouldSurvive(int x, int y)
		{
			if (x == 0 && y == 0)
				foodEatenThisRound = 0;

			bool isAlive = this[x, y];
			if (this[x, y])
				aliveCount[x, y]++;
			else if (aliveCount[x, y] > 0)
				aliveCount[x, y]--; //ncrunch: no coverage

			int neighbours = GetNumberOfNeighbours(x, y);
			bool isStarving = foodEatenThisRound > foodAvailablePerRound;
			bool isSurviving = isStarving == false && aliveCount[x, y] < MaxAge &&
				neighbours > (isAlive ? aliveStrategy[x, y] : notAliveStrategy[x, y]);
			if (isSurviving == false)
			{
				if (isAlive)
					aliveStrategy[x, y] =  random.Get(MinNeighbours, neighbours);
				else
					notAliveStrategy[x, y] = random.Get(neighbours, MaxNeighbours);
			}
			if (isSurviving)
				foodEatenThisRound++;
			return isSurviving;
		}

		private int foodEatenThisRound;
		private const int MaxAge = 10;
	}
}
