using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class ControlTests : TestWithAllFrameworks
	{
		[Test]
		public void SetControls()
		{
			Start(typeof(MockResolver),
				(InputCommands input, ContentLoader contentLoader, ScreenSpace screenSpace) =>
				{
					new AsteroidsGame(contentLoader, input, screenSpace);
					mockResolver.input.SetKeyboardState(Key.W, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.A, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.D, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
					mockResolver.AdvanceTimeAndExecuteRunners();
					mockResolver.input.SetKeyboardState(Key.Space, State.Releasing);
					mockResolver.AdvanceTimeAndExecuteRunners();
				});
		}
	}
}