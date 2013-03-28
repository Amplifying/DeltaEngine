using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadTests : TestStarter
	{
		[IntegrationTest]
		public void UpdateGamepad(Type resolver)
		{
			Start(resolver, (GamePad gamepad) =>
			{
				gamepad.Run();
				Assert.True(gamepad.IsAvailable);
				Assert.AreEqual(gamepad.GetButtonState(GamePadButton.Up), State.Released);
				Assert.AreEqual(gamepad.GetButtonState(GamePadButton.X), State.Released);
			});
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			Rect rect = null;
			Start(resolver, (Renderer renderer) =>
			{
				rect = new Rect(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.GetRandomBrightColor());
				renderer.Add(rect);
			}, (GamePad gamepad) =>
			{
				rect.DrawArea.TopLeft = gamepad.GetButtonState(GamePadButton.X) == State.Pressed
					? Point.Half : Point.Zero;
				rect.Color = Color.GetRandomBrightColor();
			});
		}

		[VisualTest]
		public void CheckLeftTrigger(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => { }, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "LeftTrigger=" + gamepad.GetLeftTrigger();
			});
		}

		[VisualTest]
		public void CheckLeftThumb(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => { }, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "LeftThumb=" + gamepad.GetLeftThumbStick();
			});
		}

		[VisualTest]
		public void CheckRightTrigger(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => { }, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "RightTrigger=" + gamepad.GetRightTrigger();
			});
		}

		[VisualTest]
		public void CheckRightThumb(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => { }, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "RightThumb=" + gamepad.GetRightThumbStick();
			});
		}
	}
}