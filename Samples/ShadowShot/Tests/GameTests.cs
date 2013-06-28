using System;
using DeltaEngine.Content;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace ShadowShot.Tests
{
	public class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void DisplayGameWindow(Type resolver)
		{
			var game = new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			Assert.AreEqual(ContentLoader.Load<Image>("starfield"), game.Background.Image);
			Assert.AreEqual((int)Constants.RenderLayer.Background, game.Background.RenderLayer);
		}

		[Test]
		public void CheckPlayerShipMoveLeft()
		{
			var game = new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.A, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.CursorLeft, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.A, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.CursorLeft, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		[Test]
		public void CheckPlayerShipMoveRight()
		{
			var game = new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.D, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.CursorRight, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.D, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.CursorRight, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		[Test]
		public void StopPlayerShipMovement()
		{
			var game = new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.S, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.CursorDown, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.S, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
			keyboard.SetKeyboardState(Key.CursorDown, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
		}
	}
}