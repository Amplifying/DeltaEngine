using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class BaseMouseTests
	{
		[Test]
		public void GetButtonState()
		{
			MockMouse mouse = new MockMouse();
			mouse.SetState(MouseButton.Left, State.Released);
			mouse.SetState(MouseButton.Right, State.Pressing);
			mouse.SetState(MouseButton.Middle, State.Pressed);
			mouse.SetState(MouseButton.X1, State.Released);
			mouse.SetState(MouseButton.X2, State.Released);

			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Left));
			Assert.AreEqual(State.Pressing, mouse.GetButtonState(MouseButton.Right));
			Assert.AreEqual(State.Pressed, mouse.GetButtonState(MouseButton.Middle));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X1));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X2));
		}

		private class MockMouse : BaseMouse
		{
			public void SetState(MouseButton button, State state)
			{
				if (button == MouseButton.Left)
					LeftButton = state;
				if (button == MouseButton.Right)
					RightButton = state;
				if (button == MouseButton.Middle)
					MiddleButton = state;
				if (button == MouseButton.X1)
					X1Button = state;
				if (button == MouseButton.X2)
					X2Button = state;
			}

			//ncrunch: no coverage start
			public override bool IsAvailable
			{
				get { return true; }
			}

			public override void SetPosition(Point newPosition) {}

			public override void Run() {}

			public override void Dispose() {}
		}
	}
}