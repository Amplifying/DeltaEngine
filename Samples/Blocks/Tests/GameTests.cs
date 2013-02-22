using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Game
	/// </summary>
	public class GameTests : TestStarter
	{
		[Test]
		public void ConstructorRendersScoreboard()
		{
			Start(typeof(TestResolver),
				(Game game, Renderer renderer) =>
					Assert.AreEqual(5, renderer.NumberOfActiveRenderableObjects));
		}

		[Test]
		public void AffixingBlockAddsToScore()
		{
			var resolver = new TestResolver();
			resolver.Register<ModdableContent>();
			resolver.RegisterSingleton<Grid>();
			var game = resolver.Resolve<TestGame>();
			Assert.AreEqual(0, game.Score);
			resolver.AdvanceTimeAndExecuteRunners(10.0f);
			Assert.AreEqual(1, game.Score);
			Assert.AreEqual("Score 1", game.Scoreboard.Text);
		}

		[Test]
		public void CursorLeftMovesBlockLeft()
		{
			Start(typeof(TestResolver), (Game game, TestController controller, Content content) =>
			{
				controller.SetUpcomingBlock(new Block(content, new FixedRandom(), Point.Zero));
				controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(6, 1)));
				testResolver.SetKeyboardState(Key.CursorLeft, State.Pressing);
				testResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(5, controller.FallingBlock.Left);
			});
		}

		[Test]
		public void CursorRightMovesBlockRight()
		{
			Start(typeof(TestResolver), (Game game, TestController controller, Content content) =>
			{
				controller.SetUpcomingBlock(new Block(content, new FixedRandom(), Point.Zero));
				controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(6, 1)));
				testResolver.SetKeyboardState(Key.CursorRight, State.Pressing);
				testResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(7, controller.FallingBlock.Left);
			});
		}

		[Test]
		public void CursorUpRotatesBlock()
		{
			Start(typeof(TestResolver), (Game game, TestController controller, Content content) =>
			{
				controller.SetUpcomingBlock(new Block(content, new FixedRandom(), Point.Zero));
				controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(6, 1)));
				testResolver.SetKeyboardState(Key.CursorUp, State.Pressing);
				Assert.AreEqual("OOOO/..../..../....", controller.FallingBlock.ToString());
				testResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual("O.../O.../O.../O...", controller.FallingBlock.ToString());
			});
		}

		[Test]
		public void LosingResetsScore()
		{
			var resolver = new TestResolver();
			resolver.RegisterSingleton<Grid>();
			resolver.RegisterSingleton<TestController>();
			var game = resolver.Resolve<TestGame>();
			var controller = resolver.Resolve<TestController>();
			var content = resolver.Resolve<ModdableContent>();
			var grid = resolver.Resolve<Grid>();

			controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(0, 20)));
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreEqual(1, game.Score);
			grid.AffixBlock(new Block(content, new FixedRandom(), new Point(0, 1)));
			controller.SetFallingBlock(new Block(content, new FixedRandom(), new Point(4, 20)));
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreEqual(0, game.Score);
			Assert.AreEqual("Final Score 2", game.Scoreboard.Text);
		}

		[VisualTest]
		public void PlayGame(Type resolver)
		{
			Start(resolver, (Game game) => { });
		}
	}
}