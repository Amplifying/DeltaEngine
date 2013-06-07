using System;
using DeltaEngine.Content;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using ShadowShotGame;

namespace ShadowShotGameTests
{
	public class GameTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void DisplayGameWindow(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, InputCommands input, ContentLoader content) =>
			{
				var game = new Game(screen, input, content);
				Assert.AreEqual(content.Load<Image>("starfield"), game.Background.Image);
				Assert.AreEqual((int)Constants.RenderLayer.Background, game.Background.RenderLayer);
			});
		}

		[Test]
		public void CheckPlayerShipMoveLeft()
		{
			Start(typeof(MockResolver),
				(ScreenSpace screen, InputCommands input, ContentLoader content) =>
				{
					new Game(screen, input, content);
					mockResolver.input.SetKeyboardState(Key.A, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.A, State.Pressing);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Pressing);
					mockResolver.AdvanceTimeAndExecuteRunners();
				});
		}

		[Test]
		public void CheckPlayerShipMoveRight()
		{
			Start(typeof(MockResolver),
				(ScreenSpace screen, InputCommands input, ContentLoader content) =>
				{
					new Game(screen, input, content);
					mockResolver.input.SetKeyboardState(Key.D, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.CursorRight, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.D, State.Pressing);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.CursorRight, State.Pressing);
					mockResolver.AdvanceTimeAndExecuteRunners();
				});
		}

		[Test]
		public void StopPlayerShipMovement()
		{
			Start(typeof(MockResolver),
				(ScreenSpace screen, InputCommands input, ContentLoader content) =>
				{
					new Game(screen, input, content);
					mockResolver.input.SetKeyboardState(Key.S, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.CursorDown, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.S, State.Pressing);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.CursorDown, State.Pressing);
					mockResolver.AdvanceTimeAndExecuteRunners();
				});
		}
	}
}