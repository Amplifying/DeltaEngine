using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
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
			ColoredRectangle rect = null;
			GamePad remGamepad = null;

			Start(resolver, (Renderer renderer, GamePad gamepad) =>
			{
				remGamepad = gamepad;
				rect = new ColoredRectangle(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f),
					Color.GetRandomBrightColor());
				renderer.Add(rect);
			}, delegate
			{
				var position = remGamepad.GetButtonState(GamePadButton.X) == State.IsPressed
					? Point.Half : Point.Zero;
				rect.DrawArea.Left = position.X;
				rect.DrawArea.Top = position.Y;
				rect.Color = Color.GetRandomBrightColor();
			});
		}

		[VisualTest]
		public void CheckRanges(Type resolver)
		{
			Window remWindow = null;
			GamePad remGamepad = null;
			Start(resolver, (GamePad gamepad, Window window) =>
			{
				remWindow = window;
				remGamepad = gamepad;
			}, () =>
			{
				remGamepad.Run();
				remWindow.Title = "LeftTrigger=" + remGamepad.GetLeftTrigger() + " - LeftThumb=" +
					remGamepad.GetLeftThumbStick();
			});
		}
	}
}