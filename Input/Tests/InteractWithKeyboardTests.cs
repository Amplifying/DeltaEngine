using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InteractWithKeyboardTests : TestWithMocksOrVisually
	{
		[Test]
		public void PressKey()
		{
			//TODO: this should not be an Entity2D
			var entity = new Entity2D(Rectangle.Zero).Start<InteractWithKeyboard>();
			var keypress = Key.None;
			entity.Messaged += message =>
			{
				var keyPressed = message as InteractWithKeyboard.KeyPress;
				if (keyPressed != null)
					keypress = keyPressed.Key;
			};
			PressKey(Key.A, State.Releasing);
			Assert.AreEqual(Key.A, keypress);
		}

		private void PressKey(Key key, State state)
		{
			resolver.AdvanceTimeAndExecuteRunners();
			Resolve<MockKeyboard>().SetKeyboardState(key, state);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		[Test]
		public void PressingTwoKeysTogetherSendsTwoMessages()
		{
			//TODO: this should not be an Entity2D
			var entity = new Entity2D(Rectangle.Zero).Start<InteractWithKeyboard>();
			int count = 0;
			entity.Messaged += message =>
			{
				var keyPressed = message as InteractWithKeyboard.KeyPress;
				if (keyPressed != null)
					count++;
			};
			PressTwoKeys();
			Assert.AreEqual(2, count);
		}

		private void PressTwoKeys()
		{
			resolver.AdvanceTimeAndExecuteRunners();
			Resolve<MockKeyboard>().SetKeyboardState(Key.A, State.Releasing);
			Resolve<MockKeyboard>().SetKeyboardState(Key.B, State.Releasing);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		[Test]
		public void HoldKey()
		{
			//TODO: this should not be an Entity2D
			var entity = new Entity2D(Rectangle.Zero).Start<InteractWithKeyboard>();
			var keypress = Key.None;
			entity.Messaged += message =>
			{
				var keyHeld = message as InteractWithKeyboard.KeyHeld;
				if (keyHeld != null)
					keypress = keyHeld.Key;
			};
			PressKey(Key.A, State.Pressed);
			Assert.AreEqual(Key.A, keypress);
		}
	}
}