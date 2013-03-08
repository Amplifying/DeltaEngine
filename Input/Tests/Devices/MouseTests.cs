using System;
using System.Threading;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests.Devices
{
	public class MouseTests : TestStarter
	{
		[IntegrationTest]
		public void UpdateMouse(Type resolver)
		{
			Start(resolver, (Mouse mouse) =>
			{
				Assert.True(mouse.IsAvailable);
				Assert.AreEqual(State.Released, mouse.MiddleButton);
				Assert.AreEqual(Point.Half, mouse.Position);
				mouse.SetPosition(new Point(0f, 0.3f));
				mouse.Run();
				Assert.AreEqual(new Point(0f, 0.3f), mouse.Position);
			});
		}

		[Test]
		public void GetButtonState()
		{
			var mouse = new TestResolver().Resolve<Mouse>();
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Left));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Middle));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Right));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X1));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X2));
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			Rect rect = null;
			Mouse remMouse = null;

			Start(resolver, (Renderer renderer, Mouse mouse, Window window) =>
			{
				window.Title = "Click to draw";
				remMouse = mouse;
				rect = new Rect(new Rectangle(-0.1f, -0.1f, 0.1f, 0.1f), Color.Red);
				renderer.Add(rect);
			}, delegate
			{
				var position = remMouse.LeftButton == State.Pressed ? remMouse.Position : -Point.Half;
				rect.DrawArea.Left = position.X;
				rect.DrawArea.Top = position.Y;
			});
		}

		/// <summary>
		/// Simulates a very low frame situation where the mouse input is only checked every 0.5
		/// seconds. Only WindowsMouse is event based and will correctly figure press+releases in
		/// between frames. Always use WindowsMouse in low fps situations.
		/// </summary>
		[VisualTest]
		public void DisplayCurrentStateWithTwoFps(Type resolver)
		{
			Start(resolver, (Window window) => { }, (Window window, InputCommands input) =>
			{
				window.Title = "MouseLeft: " + input.mouse.GetButtonState(MouseButton.Left);
				Thread.Sleep(500);
			});
		}

		[VisualTest]
		public void CountPressingAndReleasing(Type resolver)
		{
			Start(resolver, (Window window, InputCommands input) =>
			{
				int pressed = 0;
				int released = 0;
				input.Add(MouseButton.Left, State.Pressing,
					trigger => window.Title = "MouseLeft pressed: " + ++pressed + " released: " + released);
				input.Add(MouseButton.Left, State.Releasing,
					trigger => window.Title = "MouseLeft pressed: " + pressed + " released: " + ++released);

				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
					testResolver.AdvanceTimeAndExecuteRunners(1);
					testResolver.SetMouseButtonState(MouseButton.Left, State.Releasing, Point.Half);
				}
			});
		}
	}
}