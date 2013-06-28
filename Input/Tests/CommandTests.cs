using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class CommandTests : TestWithMocksOrVisually
	{
		[Test]
		public void AddAndRemoveTrigger()
		{
			var command = new Command();
			var trigger = new MouseButtonTrigger(MouseButton.Left, State.Releasing);
			command.Add(trigger);
			Assert.AreEqual(1, command.NumberOfAttachedTriggers);
			command.Remove(trigger);
			Assert.AreEqual(0, command.NumberOfAttachedTriggers);
		}

		[Test]
		public void SimulateMouseMovement()
		{
			var command = new Command();
			Resolve<InputCommands>().Add(command);
			command.Add(new MouseMovementTrigger());
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.Half);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.One);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.IsTrue(command.TriggerFired);
		}

		[Test]
		public void SimulateTouch()
		{
			var command = new Command();
			Resolve<InputCommands>().Add(command);
			command.Add(new TouchPressTrigger(State.Pressed));
			MockTouch.SetTouchState(0, State.Pressed, Point.Half);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.IsTrue(command.TriggerFired);
		}

		[Test]
		public void SimulateKeyPress()
		{
			SimulateKeyOrMousePress(Resolve<InputCommands>(), true);
		}

		[Test]
		public void SimulateGamePad()
		{
			var command = new Command();
			Resolve<InputCommands>().Add(command);
			command.Add(new GamePadButtonTrigger(GamePadButton.A, State.Pressed));
			Resolve<MockGamePad>().SetGamePadState(GamePadButton.A, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.IsTrue(command.TriggerFired);
			foreach (var buttonTrigger in command.GetTriggers().Cast<GamePadButtonTrigger>())
			{
				Assert.AreEqual(GamePadButton.A, buttonTrigger.Button);
				Assert.AreEqual(State.Pressed, buttonTrigger.State);
			}
		}

		private void SimulateKeyOrMousePress(InputCommands input, bool key)
		{
			var command = new Command();
			InputIsKeyOrMouse(input, key, command);
			bool triggered = false;
			command.Attach(trigger => triggered = true);
			if (key)
				Resolve<MockKeyboard>().SetKeyboardState(Key.A, State.Pressed);
			else
				Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, State.Releasing);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.IsTrue(triggered);
			CheckTriggers(command, key);
		}

		private static void InputIsKeyOrMouse(InputCommands input, bool key, Command command)
		{
			input.Add(command);
			if (key)
				command.Add(new KeyTrigger(Key.A, State.Pressed));
			else
				command.Add(new MouseButtonTrigger(MouseButton.Left, State.Releasing));
		}

		private static void CheckTriggers(Command command, bool key)
		{
			if (key)
				foreach (var keyTrigger in command.GetTriggers().Cast<KeyTrigger>())
				{
					Assert.AreEqual(Key.A, keyTrigger.Key);
					Assert.AreEqual(State.Pressed, keyTrigger.State);
				}
			else
				foreach (var buttonTrigger in command.GetTriggers().Cast<MouseButtonTrigger>())
				{
					Assert.AreEqual(MouseButton.Left, buttonTrigger.Button);
					Assert.AreEqual(State.Releasing, buttonTrigger.State);
				}
		}

		[Test]
		public void Run()
		{
			var command = new Command();
			command.Add(new KeyTrigger(Key.Y, State.Releasing));
			command.Run(Resolve<InputCommands>());
		}

		[Test]
		public void ConditionMatched()
		{
			var command = new Command();
			command.Add(new KeyTrigger(Key.Y, State.Released));
			command.Run(Resolve<InputCommands>());
		}

		[Test]
		public void GraphicalUnitTest()
		{
			var currentPosition = new Point(0.1f, 0.1f);
			var ellipse = new Ellipse(new Rectangle(Point.Zero, new Size(0.1f, 0.1f)), Color.Red);
			Resolve<InputCommands>().Add(Key.A, State.Pressing,
				key => currentPosition = new Point(0.6f, 0.5f));
			Resolve<InputCommands>().Add(Key.A, State.Released,
				key => currentPosition = new Point(0.1f, 0.1f));
			RunCode = () => ellipse.Center = currentPosition;
		}
	}
}