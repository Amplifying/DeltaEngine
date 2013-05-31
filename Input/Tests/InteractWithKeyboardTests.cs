using System;
using DeltaEngine.Entities.Tests;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InteractWithKeyboardTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void PressKey(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = new EmptyEntity().Add<InteractWithKeyboard>();
				var keypress = Key.None;
				entity.Messaged += key => keypress = ((InteractWithKeyboard.KeyPress)key).Key;
				PressKey(Key.A);
				Assert.AreEqual(Key.A, keypress);
			});
		}

		private void PressKey(Key key)
		{
			mockResolver.AdvanceTimeAndExecuteRunners();
			mockResolver.input.SetKeyboardState(key, State.Releasing);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}

		[IntegrationTest]
		public void PressingTwoKeysTogetherSendsTwoMessages(Type resolver)
		{
			Start(resolver, () =>
			{
				var entity = new EmptyEntity().Add<InteractWithKeyboard>();
				int count = 0;
				entity.Messaged += key => count++;
				PressTwoKeys();
				Assert.AreEqual(2, count);
			});
		}

		private void PressTwoKeys()
		{
			mockResolver.AdvanceTimeAndExecuteRunners();
			mockResolver.input.SetKeyboardState(Key.A, State.Releasing);
			mockResolver.input.SetKeyboardState(Key.B, State.Releasing);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}
	}
}