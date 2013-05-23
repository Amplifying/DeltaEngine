using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class KeyboardTests : TestWithAllFrameworks
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
			Ellipse ellipse = null;
			Keyboard remKeyboard = null;

			Start(resolver, (EntitySystem entitySystem, Keyboard keyboard) =>
			{
				remKeyboard = keyboard;
				ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.GetRandomBrightColor());
				entitySystem.Add(ellipse);
			}, delegate
			{
				var position = remKeyboard.GetKeyState(Key.A) == State.Pressed ? Point.Half : Point.Zero;
				var drawArea = ellipse.DrawArea;
				drawArea.Left = position.X;
				drawArea.Top = position.Y;
				ellipse.DrawArea = drawArea;
				ellipse.Color = Color.GetRandomBrightColor();
			});
		}
	}
}