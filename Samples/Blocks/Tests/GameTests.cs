using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Game
	/// </summary>
	public class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateGameInPortrait()
		{
			var window = Resolve<Window>();
			Initialize(Resolve<ScreenSpace>());
			window.ViewportPixelSize = new Size(600, 800);
			EntitySystem.Current.Run();
		}

		private void Initialize(ScreenSpace screen)
		{
			displayMode = screen.Viewport.Aspect >= 1.0f ? Orientation.Landscape : Orientation.Portrait;
			fixedRandomScope = Randomizer.Use(new FixedRandom());
		}

		private Orientation displayMode;
		private IDisposable fixedRandomScope;

		[Test]
		public void AffixingBlockAddsToScore()
		{
			var screenSpace = Resolve<ScreenSpace>();
			var game = new Game(screenSpace, Resolve<InputCommands>(), new JewelBlocksContent(),
				Resolve<SoundDevice>());
			Initialize(screenSpace);
			Assert.AreEqual(0, game.UserInterface.Score);
			resolver.AdvanceTimeAndExecuteRunners(10.0f);
			Assert.AreEqual(1, game.UserInterface.Score);
		}

		[Test]
		public void CursorLeftMovesBlockLeft()
		{
			var screenSpace = Resolve<ScreenSpace>();
			var content = new JewelBlocksContent();
			var game = new Game(screenSpace, Resolve<InputCommands>(), content, Resolve<SoundDevice>());
			InitializeBlocks(screenSpace, game.Controller, content);
			//mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(6, game.Controller.FallingBlock.Left);
		}

		private void InitializeBlocks(ScreenSpace screenSpace, Controller controller,
			JewelBlocksContent content)
		{
			Initialize(screenSpace);
			controller.UpcomingBlock = new Block(displayMode, content, Point.Zero);
			controller.FallingBlock = new Block(displayMode, content, new Point(6, 1));
		}

		[Test]
		public void HoldingCursorLeftEventuallyMovesBlockLeftTwice()
		{
			var screenSpace = Resolve<ScreenSpace>();
			var content = new JewelBlocksContent();
			var game = new Game(screenSpace, Resolve<InputCommands>(), content, Resolve<SoundDevice>());
			InitializeBlocks(screenSpace, game.Controller, content);
			//mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			//mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreEqual(6, game.Controller.FallingBlock.Left);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreEqual(6, game.Controller.FallingBlock.Left);
		}

		[Test]
		public void CursorRightMovesBlockRight()
		{
			var screenSpace = Resolve<ScreenSpace>();
			var content = new JewelBlocksContent();
			var game = new Game(screenSpace, Resolve<InputCommands>(), content, Resolve<SoundDevice>());
			InitializeBlocks(screenSpace, game.Controller, content);
			//mockResolver.input.SetKeyboardState(Key.CursorRight, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(6, game.Controller.FallingBlock.Left);
		}

//		[Test]
//		public void HoldingCursorRightEventuallyMovesBlockRightTwice()
//		{
//			InitializeBlocks(screenSpace, game.Controller, content);
//			mockResolver.input.SetKeyboardState(Key.CursorRight, State.Pressing);
//			mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//			mockResolver.input.SetKeyboardState(Key.CursorRight, State.Pressed);
//			mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
//			Assert.AreEqual(7, game.Controller.FallingBlock.Left);
//			mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
//			Assert.AreEqual(8, game.Controller.FallingBlock.Left);
//		}

//		[Test]
//		public void CursorUpRotatesBlock()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					using (Randomizer.Use(new FixedRandom()))
//					{
//						InitializeBlocks(screenSpace, game.Controller, content);
//						mockResolver.input.SetKeyboardState(Key.CursorUp, State.Pressing);
//						Assert.AreEqual("OOOO/..../..../....", game.Controller.FallingBlock.ToString());
//						mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//						Assert.AreEqual("O.../O.../O.../O...", game.Controller.FallingBlock.ToString());
//					}
//				});
//		}

//		[Test]
//		public void CursorDownDropsBlockFast()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					Assert.IsFalse(game.Controller.IsFallingFast);
//					mockResolver.input.SetKeyboardState(Key.CursorDown, State.Pressing);
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.IsTrue(game.Controller.IsFallingFast);
//					mockResolver.input.SetKeyboardState(Key.CursorDown, State.Releasing);
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.IsFalse(game.Controller.IsFallingFast);
//				});
//		}

//		[Test]
//		public void LeftHalfClickMovesBlockLeft()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					mockResolver.input.SetMousePosition(new Point(0.35f, 0.0f));
//					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.AreEqual(5, game.Controller.FallingBlock.Left);
//				});
//		}

//		[Test]
//		public void RightHalfClickMovesBlockRight()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					mockResolver.input.SetMousePosition(new Point(0.65f, 0.0f));
//					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.AreEqual(7, game.Controller.FallingBlock.Left);
//				});
//		}

//		[Test]
//		public void TopHalfClickRotatesBlock()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					Assert.AreEqual("OOOO/..../..../....", game.Controller.FallingBlock.ToString());
//					mockResolver.input.SetMousePosition(new Point(0.5f, 0.4f));
//					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.AreEqual("O.../O.../O.../O...", game.Controller.FallingBlock.ToString());
//				});
//		}

//		[Test]
//		public void BottomHalfClickDropsBlockFast()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					Assert.IsFalse(game.Controller.IsFallingFast);
//					mockResolver.input.SetMousePosition(new Point(0.5f, 0.6f));
//					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.IsTrue(game.Controller.IsFallingFast);
//					mockResolver.input.SetMousePosition(new Point(0.5f, 0.6f));
//					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Releasing);
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.IsFalse(game.Controller.IsFallingFast);
//				});
//		}

//		[Test]
//		public void LeftHalfTouchMovesBlockLeft()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					mockResolver.input.SetTouchState(0, State.Pressing, new Point(0.35f, 0.0f));
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.AreEqual(5, game.Controller.FallingBlock.Left);
//				});
//		}

//		[Test]
//		public void RightHalfTouchMovesBlockRight()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					mockResolver.input.SetTouchState(0, State.Pressing, new Point(0.65f, 0.0f));
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.AreEqual(7, game.Controller.FallingBlock.Left);
//				});
//		}

//		[Test]
//		public void TopHalfTouchRotatesBlock()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					Assert.AreEqual("OOOO/..../..../....", game.Controller.FallingBlock.ToString());
//					mockResolver.input.SetTouchState(0, State.Pressing, new Point(0.5f, 0.4f));
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.AreEqual("O.../O.../O.../O...", game.Controller.FallingBlock.ToString());
//				});
//		}

//		[Test]
//		public void BottomHalfTouchDropsBlockFast()
//		{
//			Start(typeof(MockResolver),
//				(ScreenSpace screenSpace, Game game, JewelBlocksContent content) =>
//				{
//					InitializeBlocks(screenSpace, game.Controller, content);
//					Assert.IsFalse(game.Controller.IsFallingFast);
//					mockResolver.input.SetTouchState(0, State.Pressing, new Point(0.5f, 0.6f));
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.IsTrue(game.Controller.IsFallingFast);
//					mockResolver.input.SetTouchState(0, State.Releasing, new Point(0.5f, 0.6f));
//					mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
//					Assert.IsFalse(game.Controller.IsFallingFast);
//				});
//		}

//		[TestFixtureTearDown]
//		public void TearDown()
//		{
//			if (fixedRandomScope != null)
//				fixedRandomScope.Dispose();
//		}
	}
}