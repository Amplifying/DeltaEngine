using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace SideScrollerSample.Tests
{
	internal class GameControlsTests : TestWithAllFrameworks
	{
		private void CreateGameControls(InputCommands inputCommands)
		{
			gameControls = new GameControls(inputCommands);
		}

		private GameControls gameControls;

		[Test]
		public void TestAscendControls()
		{
			Start(typeof(MockResolver), (InputCommands inputCommands) =>
			{
				CreateGameControls(inputCommands);
				bool ascended = false;
				gameControls.Ascend += () => { ascended = true; };
				mockResolver.input.SetKeyboardState(Key.W, State.Pressed);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.IsTrue(ascended);
			});
		}

		[Test]
		public void TestSinkControls()
		{
			Start(typeof(MockResolver), (InputCommands inputCommands) =>
			{
				CreateGameControls(inputCommands);
				bool sinking = false;
				gameControls.Sink += () => { sinking = true; };
				mockResolver.input.SetKeyboardState(Key.S, State.Pressed);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.IsTrue(sinking);
			});
		}

		[Test]
		public void TestAccelerateControls()
		{
			Start(typeof(MockResolver), (InputCommands inputCommands) =>
			{
				CreateGameControls(inputCommands);
				bool accelerated = false;
				gameControls.Accelerate += () => { accelerated = true; };
				mockResolver.input.SetKeyboardState(Key.D, State.Pressed);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.IsTrue(accelerated);
			});
		}

		[Test]
		public void TestSlowDownControls()
		{
			Start(typeof(MockResolver), (InputCommands inputCommands) =>
			{
				CreateGameControls(inputCommands);
				bool slowingDown = false;
				gameControls.SlowDown += () => { slowingDown = true; };
				mockResolver.input.SetKeyboardState(Key.A, State.Pressed);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.IsTrue(slowingDown);
			});
		}

		[Test]
		public void TestShootingControls()
		{
			Start(typeof(MockResolver), (InputCommands inputCommands) =>
			{
				CreateGameControls(inputCommands);
				bool fireing = false;
				gameControls.Fire += () => { fireing = true; };
				gameControls.HoldFire += () => { fireing = false; };
				mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.IsTrue(fireing);
				mockResolver.input.SetKeyboardState(Key.Space, State.Releasing);
				mockResolver.AdvanceTimeAndExecuteRunners();
				Assert.IsFalse(fireing);
			});
		}
	}
}