using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadTests : TestWithAllFrameworks
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
			Ellipse ellipse = null;
			Start(resolver, (EntitySystem entitySystem) =>
			{
				ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.GetRandomBrightColor());
				entitySystem.Add(ellipse);
			}, (GamePad gamepad) =>
			{
				ellipse.Center = gamepad.GetButtonState(GamePadButton.X) == State.Pressed
					? Point.Half : Point.Zero;
				ellipse.Color = Color.GetRandomBrightColor();
			});
		}

		[VisualTest]
		public void CheckLeftTrigger(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => {}, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "LeftTrigger=" + gamepad.GetLeftTrigger();
			});
		}

		[VisualTest]
		public void CheckLeftThumb(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => {}, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "LeftThumb=" + gamepad.GetLeftThumbStick();
			});
		}

		[VisualTest]
		public void CheckRightTrigger(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => {}, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "RightTrigger=" + gamepad.GetRightTrigger();
			});
		}

		[VisualTest]
		public void CheckRightThumb(Type resolver)
		{
			Start(resolver, (GamePad gamepad) => {}, (GamePad gamepad, Window window) =>
			{
				gamepad.Run();
				window.Title = "RightThumb=" + gamepad.GetRightThumbStick();
			});
		}
	}
}