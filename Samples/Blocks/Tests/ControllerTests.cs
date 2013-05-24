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
	/// Unit tests for Controller
	/// </summary>
	public class ControllerTests : TestWithAllFrameworks
	{
		public void Initialize(ScreenSpace screen, ContentLoader contentLoader)
		{
			displayMode = screen.Viewport.Aspect >= 1.0f
				? Constants.DisplayMode.LandScape : Constants.DisplayMode.Portrait;
			content = new JewelBlocksContent(contentLoader);
			controller = new Controller(displayMode, content);
			sounds = controller.Get<Soundbank>();
			grid = controller.Get<Grid>();
		}

		private Constants.DisplayMode displayMode;
		private IDisposable fixedRandomScope;
		private Controller controller;
		private JewelBlocksContent content;
		private Soundbank sounds;
		private Grid grid;

		[Test]
		public void RunCreatesFallingAndUpcomingBlocks()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.IsNotNull(controller.FallingBlock);
				Assert.IsNotNull(controller.UpcomingBlock);
			});
		}

		[Test, Category("Slow")]
		public void DropSlowAffixesBlocksSlowly()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				controller.IsFallingFast = false;
				controller.FallingBlock = new Block(displayMode, content, Point.Zero);
				controller.UpcomingBlock = new Block(displayMode, content, Point.Zero);

				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.AreEqual(0, CountBricks(grid));
				mockResolver.AdvanceTimeAndExecuteRunners(1.5f);
				Assert.AreEqual(0, CountBricks(grid));
				mockResolver.AdvanceTimeAndExecuteRunners(9.0f);
				Assert.AreEqual(4, CountBricks(grid), 1);
			});
		}

		internal static int CountBricks(Grid grid)
		{
			int count = 0;
			foreach (var brick in grid.bricks)
				if (brick != null)
					count++;

			return count;
		}

		[Test, Category("Slow")]
		public void DropFastAffixesBlocksQuickly()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				controller.IsFallingFast = true;
				controller.FallingBlock = new Block(displayMode, content, Point.Zero);
				controller.UpcomingBlock = new Block(displayMode, content, Point.Zero);

				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.AreEqual(0, CountBricks(grid));
				mockResolver.AdvanceTimeAndExecuteRunners(1.4f);
				Assert.AreEqual(4, CountBricks(grid), 1);
			});
		}

		[Test, Category("Slow")]
		public void ABlockAffixingPlaysASound()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				Assert.IsFalse(sounds.BlockAffixed.IsAnyInstancePlaying);
				mockResolver.AdvanceTimeAndExecuteRunners(12.0f);
				Assert.IsTrue(sounds.BlockAffixed.IsAnyInstancePlaying);
			});
		}

		[Test, Category("Slow")]
		public void RunScoresPointsOverTime()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				int score = 0;
				controller.AddToScore += points => score += points;
				controller.FallingBlock = new Block(displayMode, content, Point.Zero);
				controller.UpcomingBlock = new Block(displayMode, content, Point.Zero);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.AreEqual(1, score);
				mockResolver.AdvanceTimeAndExecuteRunners(9.0f);
				Assert.AreEqual(2, score);
			});
		}

		[Test, Category("Slow")]
		public void WhenABlockAffixesTheUpcomingBlockBecomesTheFallingBlock()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				var upcomingBlock = controller.UpcomingBlock;
				mockResolver.AdvanceTimeAndExecuteRunners(10.0f);
				Assert.AreEqual(upcomingBlock, controller.FallingBlock);
			});
		}

		[IntegrationTest, Category("Slow")]
		public void CantMoveLeftAtLeftWall(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				Assert.IsFalse(sounds.BlockCouldntMove.IsAnyInstancePlaying);
				controller.FallingBlock = new Block(displayMode, content, new Point(0, 1));
				controller.MoveBlockLeftIfPossible();
				Assert.IsTrue(sounds.BlockCouldntMove.IsAnyInstancePlaying);
				Assert.AreEqual(0, controller.FallingBlock.Left);
			});
		}

		[IntegrationTest, Category("Slow")]
		public void CanMoveLeftElsewhere(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
				controller.FallingBlock = new Block(displayMode, content, new Point(3, 1));
				controller.MoveBlockLeftIfPossible();
				Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
				Assert.AreEqual(2, controller.FallingBlock.Left);
			});
		}

		[IntegrationTest, Category("Slow")]
		public void CantMoveRightAtRightWall(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				Assert.IsFalse(sounds.BlockCouldntMove.IsAnyInstancePlaying);
				controller.FallingBlock = new Block(displayMode, content, new Point(11, 1));
				controller.MoveBlockRightIfPossible();
				Assert.AreEqual(11, controller.FallingBlock.Left);
				Assert.IsTrue(sounds.BlockCouldntMove.IsAnyInstancePlaying);
			});
		}

		[IntegrationTest, Category("Slow")]
		public void CanMoveRightElsewhere(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
				controller.FallingBlock = new Block(displayMode, content, new Point(3, 1));
				controller.MoveBlockRightIfPossible();
				Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
				Assert.AreEqual(4, controller.FallingBlock.Left);
			});
		}

		[IntegrationTest, Category("Slow")]
		public void RotateClockwise(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				Assert.IsFalse(sounds.BlockMoved.IsAnyInstancePlaying);
				controller.FallingBlock = new Block(displayMode, content, new Point(8, 1));
				controller.RotateBlockAntiClockwiseIfPossible();
				Assert.IsTrue(sounds.BlockMoved.IsAnyInstancePlaying);
				controller.FallingBlock.Left = 11;
				controller.RotateBlockAntiClockwiseIfPossible();
			});
		}

		[Test, Category("Slow")]
		public void LoseIfIsBrickOnTopRow()
		{
			Start(typeof(MockResolver), (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				Assert.IsFalse(sounds.GameLost.IsAnyInstancePlaying);
				grid.AffixBlock(new Block(displayMode, content, new Point(1, 0)));
				bool lost = false;
				controller.Lose += () => lost = true;
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.IsTrue(lost);
				Assert.IsTrue(sounds.GameLost.IsAnyInstancePlaying);
			});
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			fixedRandomScope = Randomizer.Use(new FixedRandom());
			fixedRandomScope.Dispose();
		}
	}
}