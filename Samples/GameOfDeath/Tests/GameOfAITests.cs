using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;

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
}
