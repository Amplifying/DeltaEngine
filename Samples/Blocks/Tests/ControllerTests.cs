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
				(BlocksContent content, TestController controller, TestGrid grid) =>
				{
					controller.IsFallingFast = false;
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.AreEqual(0, grid.BrickCount);
					testResolver.AdvanceTimeAndExecuteRunners(2.5f);
					Assert.AreEqual(0, grid.BrickCount);
					testResolver.AdvanceTimeAndExecuteRunners(9.0f);
					Assert.AreEqual(4, grid.BrickCount, 1);
				});
		}

		[Test, Category("Slow")]
		public void DropFastAffixesBlocksQuickly()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, TestGrid grid) =>
				{
					controller.IsFallingFast = true;
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.AreEqual(0, grid.BrickCount);
					testResolver.AdvanceTimeAndExecuteRunners(1.5f);
					Assert.AreEqual(4, grid.BrickCount, 1);
					testResolver.AdvanceTimeAndExecuteRunners(15.0f);
					Assert.IsTrue(grid.BrickCount > 4);
				});
		}

		[Test]
		public void ABlockAffixingPlaysASound()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, TestGrid grid) =>
				{
					Assert.IsFalse(controller.Soundbank.BlockAffixed.IsAnyInstancePlaying);
					testResolver.AdvanceTimeAndExecuteRunners(12.0f);
					Assert.IsTrue(controller.Soundbank.BlockAffixed.IsAnyInstancePlaying);
				});
		}

		[Test, Category("Slow")]
		public void RunScoresPointsOverTime()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, TestGrid grid) =>
				{
					int score = 0;
					controller.AddToScore += points => score += points;
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
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					int score = 0;
					controller.AddToScore += points => score += points;
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 18)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 18)));
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(8, 18)));
					testResolver.AdvanceTimeAndExecuteRunners(1.1f);
					Assert.AreEqual(11, score);
				});
		}

		[Test]
		public void FillingARowPlaysASound()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 18)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 18)));
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(8, 18)));
					Assert.IsFalse(controller.Soundbank.RowRemoved.IsAnyInstancePlaying);
					testResolver.AdvanceTimeAndExecuteRunners(1.1f);
					Assert.IsTrue(controller.Soundbank.RowRemoved.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void FillingTwoRowsPlaysADifferentSound()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 17)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 17)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 17)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 18)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 18)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(7, 18)));
					controller.SetFallingBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }),
						new Point(11, 15)));
					Assert.IsFalse(controller.Soundbank.MultipleRowsRemoved.IsAnyInstancePlaying);
					testResolver.AdvanceTimeAndExecuteRunners(1.1f);
					Assert.IsTrue(controller.Soundbank.MultipleRowsRemoved.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void WhenABlockAffixesTheUpcomingBlockBecomesTheFallingBlock()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, TestGrid grid) =>
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
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.Soundbank.BlockCouldntMove.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(0, 1)));
					controller.MoveBlockLeftIfPossible();
					Assert.IsTrue(controller.Soundbank.BlockCouldntMove.IsAnyInstancePlaying);
					Assert.AreEqual(0, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void CanMoveLeftElsewhere()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.Soundbank.BlockMoved.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(3, 1)));
					controller.MoveBlockLeftIfPossible();
					Assert.IsTrue(controller.Soundbank.BlockMoved.IsAnyInstancePlaying);
					Assert.AreEqual(2, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void CantMoveRightAtRightWall()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.Soundbank.BlockCouldntMove.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(8, 1)));
					controller.MoveBlockRightIfPossible();
					Assert.IsTrue(controller.Soundbank.BlockCouldntMove.IsAnyInstancePlaying);
					Assert.AreEqual(8, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void CanMoveRightElsewhere()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.Soundbank.BlockMoved.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(3, 1)));
					controller.MoveBlockRightIfPossible();
					Assert.IsTrue(controller.Soundbank.BlockMoved.IsAnyInstancePlaying);
					Assert.AreEqual(4, controller.FallingBlock.Left);
				});
		}

		[Test]
		public void RotateClockwise()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.Soundbank.BlockMoved.IsAnyInstancePlaying);
					controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(8, 1)));
					Assert.AreEqual("OOOO/..../..../....", controller.FallingBlock.ToString());
					controller.RotateBlockAntiClockwiseIfPossible();
					Assert.AreEqual("O.../O.../O.../O...", controller.FallingBlock.ToString());
					Assert.IsTrue(controller.Soundbank.BlockMoved.IsAnyInstancePlaying);
					controller.FallingBlock.Left = 11;
					controller.RotateBlockAntiClockwiseIfPossible();
					Assert.AreEqual("O.../O.../O.../O...", controller.FallingBlock.ToString());
				});
		}

		[Test]
		public void LoseIfIsBrickOnTopRow()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					Assert.IsFalse(controller.Soundbank.GameLost.IsAnyInstancePlaying);
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(1, 0)));
					bool lost = false;
					controller.Lose += () => lost = true;
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(lost);
					Assert.IsTrue(controller.Soundbank.GameLost.IsAnyInstancePlaying);
				});
		}

		[Test]
		public void LoseIfNoRoomForNewBlock()
		{
			Start(typeof(TestResolver),
				(BlocksContent content, TestController controller, Grid grid) =>
				{
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 2)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(4, 2)));
					grid.AffixBlock(new Block(content, new FixedRandom(), new Point(8, 3)));
					controller.SetUpcomingBlock(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }), Point.Zero));
					bool lost = false;
					controller.Lose += () => lost = true;
					Assert.IsFalse(lost);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(lost);
				});
		}
	}
}