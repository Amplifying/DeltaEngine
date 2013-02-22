using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Controller
	/// </summary>
	public class ControllerTests : TestStarter
	{
		[Test]
		public void RunCreatesFallingAndUpcomingBlocks()
		{
			Start(typeof(TestResolver), (Controller controller) =>
			{
				testResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.IsNotNull(controller.FallingBlock);
				Assert.IsNotNull(controller.UpcomingBlock);
			});
		}

		[Test]
		public void DropSlowAffixesBlocksSlowly()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, TestGrid grid) =>
				{
					controller.DropBlockSlow();
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.AreEqual(0, grid.BrickCount);
					testResolver.AdvanceTimeAndExecuteRunners(2.5f);
					Assert.AreEqual(0, grid.BrickCount);
					testResolver.AdvanceTimeAndExecuteRunners(9.0f);
					Assert.AreEqual(4, grid.BrickCount, 1);
				});
		}

		[Test]
		public void DropFastAffixesBlocksQuickly()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, TestGrid grid) =>
				{
					controller.DropBlockFast();
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.AreEqual(0, grid.BrickCount);
					testResolver.AdvanceTimeAndExecuteRunners(2.5f);
					Assert.AreEqual(4, grid.BrickCount, 1);
					testResolver.AdvanceTimeAndExecuteRunners(9.0f);
					Assert.IsTrue(grid.BrickCount > 4);
				});
		}

		[Test]
		public void ABlockAffixingPlaysASound()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, TestGrid grid) =>
				{
					Assert.IsFalse(controller.SoundManager.BlockAffixed.IsAnyInstancePlaying);
					testResolver.AdvanceTimeAndExecuteRunners(12.0f);
					Assert.IsTrue(controller.SoundManager.BlockAffixed.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void RunScoresPointsOverTime()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, TestGrid grid) =>
				{
					int score = 0;
					controller.ScorePoints += points => score += points;
					testResolver.AdvanceTimeAndExecuteRunners(1.0f);
					Assert.AreEqual(0, score);
					testResolver.AdvanceTimeAndExecuteRunners(9.0f);
					Assert.AreEqual(1, score);
					testResolver.AdvanceTimeAndExecuteRunners(9.0f);
					Assert.AreEqual(2, score);
				});
		}

		[Test]
		public void FillingARowAddsLotsToScore()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					int score = 0;
					controller.ScorePoints += points => score += points;
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 20)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 20)));
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(7, 20)));
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.AreEqual(11, score);
				});
		}

		[Test]
		public void FillingARowPlaysASound()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 20)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 20)));
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(7, 20)));
					Assert.IsFalse(controller.SoundManager.RowRemoved.IsAnyInstancePlaying);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(controller.SoundManager.RowRemoved.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void FillingTwoRowsPlaysADifferentSound()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 19)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 19)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(6, 19)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 20)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 20)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(6, 20)));
					controller.SetFallingBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
						new Point(10, 17)));
					Assert.IsFalse(controller.SoundManager.RowsRemoved.IsAnyInstancePlaying);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(controller.SoundManager.RowsRemoved.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void WhenABlockAffixesTheUpcomingBlockBecomesTheFallingBlock()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, TestGrid grid) =>
				{
					testResolver.AdvanceTimeAndExecuteRunners(1.0f);
					var upcomingBlock = controller.UpcomingBlock;
					testResolver.AdvanceTimeAndExecuteRunners(10.0f);
					Assert.AreEqual(upcomingBlock, controller.FallingBlock);
				});
		}

		[Test]
		public void CantMoveLeftAtLeftWall()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.SoundManager.BlockCouldntMove.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(0, 1)));
					controller.TryToMoveBlockLeft();
					Assert.IsTrue(controller.SoundManager.BlockCouldntMove.IsAnyInstancePlaying);
					Assert.AreEqual(0, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void CanMoveLeftElsewhere()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.SoundManager.BlockMoved.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(3, 1)));
					controller.TryToMoveBlockLeft();
					Assert.IsTrue(controller.SoundManager.BlockMoved.IsAnyInstancePlaying);
					Assert.AreEqual(2, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void CantMoveRightAtRightWall()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.SoundManager.BlockCouldntMove.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(7, 1)));
					controller.TryToMoveBlockRight();
					Assert.IsTrue(controller.SoundManager.BlockCouldntMove.IsAnyInstancePlaying);
					Assert.AreEqual(7, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void CanMoveRightElsewhere()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.SoundManager.BlockMoved.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(3, 1)));
					controller.TryToMoveBlockRight();
					Assert.IsTrue(controller.SoundManager.BlockMoved.IsAnyInstancePlaying);
					Assert.AreEqual(4, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void RotateClockwise()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.SoundManager.BlockMoved.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(8, 1)));
					Assert.AreEqual("OOOO/..../..../....", controller.FallingBlock.ToString());
					controller.TryToRotateBlockClockwise();
					Assert.AreEqual("O.../O.../O.../O...", controller.FallingBlock.ToString());
					Assert.IsTrue(controller.SoundManager.BlockMoved.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void LoseIfIsBrickOnTopRow()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.SoundManager.GameLost.IsAnyInstancePlaying);
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 1)));
					bool lost = false;
					controller.Lost += () => lost = true;
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(lost);
					Assert.IsTrue(controller.SoundManager.GameLost.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void LoseIfNoRoomForNewBlock()
		{
			Start(typeof(TestResolver),
				(ModdableContent content, TestController controller, Grid grid) =>
				{
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 2)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 2)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 3)));
					controller.SetUpcomingBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }), Point.Zero));
					bool lost = false;
					controller.Lost += () => lost = true;
					Assert.IsFalse(lost);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(lost);
				});
		}
	}
}