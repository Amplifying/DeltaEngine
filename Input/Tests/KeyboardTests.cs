using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class KeyboardTests : TestStarter
	{
		[IntegrationTest]
		public void UpdateKeyboard(Type resolver)
		{
			Start(resolver, (Keyboard keyboard) =>
			{
				keyboard.Run();
				Assert.True(keyboard.IsAvailable);
				Assert.AreEqual(keyboard.GetKeyState(Key.B), State.Released);
				Assert.AreEqual(keyboard.GetKeyState(Key.Enter), State.Released);
			});
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			Rect rect = null;
			Keyboard remKeyboard = null;

			Start(resolver, (Renderer renderer, Keyboard keyboard) =>
			{
				remKeyboard = keyboard;
				rect = new Rect(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f),
					Color.GetRandomBrightColor());
				renderer.Add(rect);
			}, delegate
			{
				var position = remKeyboard.GetKeyState(Key.A) == State.Pressed ? Point.Half : Point.Zero;
				rect.DrawArea.Left = position.X;
				rect.DrawArea.Top = position.Y;
				rect.Color = Color.GetRandomBrightColor();
			});
		}
	}
}
