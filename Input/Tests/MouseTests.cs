using System.Threading;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseTests : TestWithMocksOrVisually
	{
		[Test]
		public void GraphicalUnitTest()
		{
			Window.Title = "Click to show red ellipse";
			var ellipse = new Ellipse(new Rectangle(-0.1f, -0.1f, 0.1f, 0.1f), Color.Red);
			var mouse = Resolve<Mouse>();
			RunCode = () =>
			{ ellipse.Center = mouse.LeftButton == State.Pressed ? mouse.Position : -Point.Half; };
		}

		[Test]
		public void UpdateMouse()
		{
			var mouse = Resolve<MockMouse>();
			Assert.True(mouse.IsAvailable);
			Assert.AreEqual(State.Released, mouse.MiddleButton);
			mouse.SetMousePositionNextFrame(new Point(0f, 0.3f));
			mouse.Run();
			Assert.AreEqual(new Point(0f, 0.3f), mouse.Position);
		}

		[Test]
		public void GetButtonState()
		{
			var mouse = Resolve<Mouse>();
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Left));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Middle));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Right));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X1));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X2));
		}

		/// <summary>
		/// Simulates a very low frame situation where the mouse input is only checked every 0.5
		/// seconds. Only WindowsMouse is event based and will correctly figure press+releases in
		/// between frames. Always use WindowsMouse in low fps situations.
		/// </summary>
		[Test]
		public void DisplayCurrentStateWithTwoFps()
		{
			Resolve<ScreenSpace>().Window.Title = "MouseLeft: " +
				Resolve<Mouse>().GetButtonState(MouseButton.Left);
			Thread.Sleep(500);
		}

		[Test]
		public void CountPressingAndReleasing()
		{
			int pressed = 0;
			int released = 0;
			Input.Add(MouseButton.Left, State.Pressing,
				trigger => Window.Title = "MouseLeft pressed: " + ++pressed + " released: " + released);
			Input.Add(MouseButton.Left, State.Releasing,
				trigger => Window.Title = "MouseLeft pressed: " + pressed + " released: " + ++released);
			PressMouseButtonAdvanceAndPressAgain();
		}

		private void PressMouseButtonAdvanceAndPressAgain()
		{
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(1);
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, State.Releasing);
		}
	}
}