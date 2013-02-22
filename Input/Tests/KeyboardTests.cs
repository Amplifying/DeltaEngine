using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Windows;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
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

		[Test]
		public void HandleProcMessageKeyDown()
		{
			var keyboard = new WindowsKeyboard();
			keyboard.HandleProcMessage((IntPtr)Key.A, (IntPtr)0, 0);
			keyboard.Dispose();
			keyboard.Run();
			Assert.AreEqual(State.Pressing, keyboard.GetKeyState(Key.A));
		}

		[Test]
		public void HandleProcMessageKeyUp()
		{
			var keyboard = new WindowsKeyboard();
			keyboard.HandleProcMessage((IntPtr)Key.A, (IntPtr)0, 0);
			keyboard.Run();
			keyboard.HandleProcMessage((IntPtr)Key.A, (IntPtr)(1 << 0xFF), 0);
			keyboard.Dispose();
			keyboard.Run();
			Assert.AreEqual(State.Releasing, keyboard.GetKeyState(Key.A));
		}

		[Test]
		public void HandleProcMessageFullLifecycle()
		{
			var keyboard = new WindowsKeyboard();
			keyboard.HandleProcMessage((IntPtr)Key.A, (IntPtr)0, 0);
			keyboard.Run();
			Assert.AreEqual(State.Pressing, keyboard.GetKeyState(Key.A));
			keyboard.Run();
			Assert.AreEqual(State.Pressed, keyboard.GetKeyState(Key.A));
			keyboard.HandleProcMessage((IntPtr)Key.A, (IntPtr)(1 << 0xFF), 0);
			keyboard.Dispose();
			keyboard.Run();
			Assert.AreEqual(State.Releasing, keyboard.GetKeyState(Key.A));
			keyboard.Run();
			Assert.AreEqual(State.Released, keyboard.GetKeyState(Key.A));
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			ColoredRectangle rect = null;
			Keyboard remKeyboard = null;

			Start(resolver, (Renderer renderer, Keyboard keyboard) =>
			{
				remKeyboard = keyboard;
				rect = new ColoredRectangle(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f),
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
