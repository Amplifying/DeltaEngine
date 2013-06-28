using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadTests : TestWithMocksOrVisually
	{
		[Test]
		public void UpdateGamepad()
		{
			var gamepad = Resolve<GamePad>();
			if (!gamepad.IsAvailable)
				return;

			gamepad.Run();
			Assert.AreEqual(gamepad.GetButtonState(GamePadButton.Up), State.Released);
			Assert.AreEqual(gamepad.GetButtonState(GamePadButton.X), State.Released);
		}

		[Test]
		public void GraphicalUnitTest()
		{
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f),
				Color.GetRandomBrightColor());
			RunCode = () =>
			{
				ellipse.Center = Resolve<GamePad>().GetButtonState(GamePadButton.X) == State.Pressed
					? Point.Half : Point.Zero;
				ellipse.Color = Color.GetRandomBrightColor();
			};
		}

		[Test]
		public void CheckAvailable()
		{
			var gamepad = Resolve<GamePad>();
			gamepad.Run();
			Resolve<ScreenSpace>().Window.Title = "Available=" + gamepad.IsAvailable;
		}

		[Test]
		public void CheckLeftTrigger()
		{
			var gamepad = Resolve<GamePad>();
			gamepad.Run();
			Resolve<ScreenSpace>().Window.Title = "LeftTrigger=" + gamepad.GetLeftTrigger();
		}

		[Test]
		public void CheckLeftThumb()
		{
			var gamepad = Resolve<GamePad>();
			gamepad.Run();
			Resolve<ScreenSpace>().Window.Title = "LeftThumb=" + gamepad.GetLeftThumbStick();
		}

		[Test]
		public void CheckRightTrigger()
		{
			var gamepad = Resolve<GamePad>();
			gamepad.Run();
			Resolve<ScreenSpace>().Window.Title = "RightTrigger=" + gamepad.GetRightTrigger();
		}

		[Test]
		public void CheckRightThumb()
		{
			var gamepad = Resolve<GamePad>();
			gamepad.Run();
			Resolve<ScreenSpace>().Window.Title = "RightThumb=" + gamepad.GetRightThumbStick();
		}
	}
}