using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Block
	/// </summary>
	public class BlockTests : TestWithAllFrameworks
	{
		public void Initialize(ScreenSpace screen,
			ContentLoader contentLoader)
		{
			displayMode = screen.Viewport.Aspect >= 1.0f
				? Constants.DisplayMode.LandScape : Constants.DisplayMode.Portrait;
			content = new JewelBlocksContent(contentLoader);
		}

		private Constants.DisplayMode displayMode;
		private JewelBlocksContent content;

		[Test]
		public void ConstructorTopLeft()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var block = new Block(displayMode, content, new Point(1, 2));
				Assert.AreEqual(1, block.Left);
				Assert.AreEqual(2, block.Top);
			});
		}

		[Test]
		public void RotateClockwise()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				using (Randomizer.Use(new FixedRandom(JBlock)))
				{
					var block = new Block(displayMode, content, new Point(8, 1));
					Assert.AreEqual("O.../OOO./..../....", block.ToString());
					block.RotateClockwise();
					Assert.AreEqual("OO../O.../O.../....", block.ToString());
				}
			});
		}

		private static readonly float[] JBlock = new[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.9f };

		[Test]
		public void RotateAntiClockwise()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				using (Randomizer.Use(new FixedRandom(JBlock)))
				{
					var block = new Block(displayMode, content, new Point(8, 1));
					Assert.AreEqual("O.../OOO./..../....", block.ToString());
					block.RotateAntiClockwise();
					Assert.AreEqual(".O../.O../OO../....", block.ToString());
				}
			});
		}

		[Test]
		public void Left()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var shape = new Block(displayMode, content, Point.Zero) { Left = 1 };
				Assert.AreEqual(1, shape.Left);
				Assert.AreEqual(1, shape.Bricks[0].TopLeftGridCoord.X);
				Assert.AreEqual(1, shape.Bricks[1].TopLeftGridCoord.X);
				Assert.AreEqual(1, shape.Bricks[2].TopLeftGridCoord.X);
				Assert.AreEqual(1, shape.Bricks[3].TopLeftGridCoord.X);
			});
		}

		[Test]
		public void Top()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var shape = new Block(displayMode, content, Point.Zero) { Top = 1 };
				Assert.AreEqual(1, shape.Top);
				Assert.AreEqual(1, shape.Bricks[0].TopLeftGridCoord.Y);
				Assert.AreEqual(1, shape.Bricks[1].TopLeftGridCoord.Y);
				Assert.AreEqual(1, shape.Bricks[2].TopLeftGridCoord.Y);
				Assert.AreEqual(1, shape.Bricks[3].TopLeftGridCoord.Y);
			});
		}

		[Test]
		public void RunMovesTheBlock()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var block = new Block(displayMode, content, Point.Zero);
				mockResolver.AdvanceTimeAndExecuteRunners(0.0167f);
				block.UpdateBrickDrawAreas(2.0f);
				Assert.AreEqual(0.0333f, block.Top, 0.001f);
			});
		}

		[IntegrationTest, Category("Slow")]
		public void CheckIBlockAppearsATenthOfTheTime(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				int count = 0;
				for (int i = 0; i < 10000; i++)
				{
					var block = new Block(displayMode, content, Point.Zero);
					if (block.ToString() == "OOOO/..../..../...." || block.ToString() == "O.../O.../O.../O...")
						count++;
				}

				Assert.AreEqual(1000, count, 100);
			});
		}

		[VisualTest]
		public void RenderJBlock(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				using (Randomizer.Use(new FixedRandom(JBlock)))
				{
					var block = new Block(displayMode, content, Point.Zero);
					block.UpdateBrickDrawAreas(0.0f);
				}
			});
		}
	}
}