using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace SideScroller.Tests
{
	internal class GameControlsTests : TestWithMocksOrVisually
	{
		private void CreateGameControls(InputCommands inputCommands)
		{
			gameControls = new GameControls(inputCommands);
		}

		private GameControls gameControls;

		[Test]
		public void TestAscendControls()
		{
			CreateGameControls(Resolve<InputCommands>());
			bool ascended = false;
			gameControls.Ascend += () => { ascended = true; };
			if(resolver.GetType() != typeof(MockResolver))
				return;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.W, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.IsTrue(ascended);

		}

		[Test]
		public void TestSinkControls()
		{
			CreateGameControls(Resolve<InputCommands>());
			bool sinking = false;
			gameControls.Sink += () => { sinking = true; };
			if (resolver.GetType() != typeof(MockResolver))
				return;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.S, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.IsTrue(sinking);

		}

		[Test]
		public void TestAccelerateControls()
		{

			CreateGameControls(Resolve<InputCommands>());
			bool accelerated = false;
			gameControls.Accelerate += () => { accelerated = true; };
			if (resolver.GetType() != typeof(MockResolver))
				return;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.D, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.IsTrue(accelerated);

		}

		[Test]
		public void TestSlowDownControls()
		{

			CreateGameControls(Resolve<InputCommands>());
			bool slowingDown = false;
			gameControls.SlowDown += () => { slowingDown = true; };
			if (resolver.GetType() != typeof(MockResolver))
				return;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.A, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.IsTrue(slowingDown);

		}

		[Test]
		public void TestShootingControls()
		{
			CreateGameControls(Resolve<InputCommands>());
			bool fireing = false;
			gameControls.Fire += () => { fireing = true; };
			gameControls.HoldFire += () => { fireing = false; };
			if (resolver.GetType() != typeof(MockResolver))
				return;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.Space, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.IsTrue(fireing);
			keyboard.SetKeyboardState(Key.Space, State.Releasing);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.IsFalse(fireing);
		}
	}
}