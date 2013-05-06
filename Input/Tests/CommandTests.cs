using System;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class CommandTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void AddAndRemoveTrigger(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				var trigger = new MouseButtonTrigger(MouseButton.Left, State.Releasing);
				command.Add(trigger);
				Assert.AreEqual(1, command.NumberOfAttachedTriggers);
				command.Remove(trigger);
				Assert.AreEqual(0, command.NumberOfAttachedTriggers);
			});
		}

		[IntegrationTest]
		public void SimulateMouseClick(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				SimulateKeyOrMousePress(input, false);
			});
		}

		[IntegrationTest]
		public void SimulateMouseMovement(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				input.Add(command);
				command.Add(new MouseMovementTrigger());
				if (mockResolver != null)
				{
					mockResolver.input.SetMousePosition(Point.Half);
					mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
					mockResolver.input.SetMousePosition(Point.One);
					mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(command.TriggerFired);
				}
			});
		}

		[IntegrationTest]
		public void SimulateTouch(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				input.Add(command);
				command.Add(new TouchPressTrigger(State.Pressed));
				if (mockResolver != null)
				{
					mockResolver.input.SetTouchState(0, State.Pressed, Point.Half);
					mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(command.TriggerFired);
				}
			});
		}

		[IntegrationTest]
		public void SimulateKeyPress(Type resolver)
		{
			Start(resolver, (InputCommands input) => SimulateKeyOrMousePress(input, true));
		}

		[IntegrationTest]
		public void SimulateGamePad(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				input.Add(command);
				command.Add(new GamePadButtonTrigger(GamePadButton.A, State.Pressed));
				if (mockResolver != null)
				{
					mockResolver.input.SetGamePadState(GamePadButton.A, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(command.TriggerFired);
					foreach (var buttonTrigger in command.GetTriggers().Cast<GamePadButtonTrigger>())
					{
						Assert.AreEqual(GamePadButton.A, buttonTrigger.Button);
						Assert.AreEqual(State.Pressed, buttonTrigger.State);
					}
				}
			});
		}

		private void SimulateKeyOrMousePress(InputCommands input, bool key)
		{
			var command = new Command();
			InputIsKeyOrMouse(input, key, command);
			bool triggered = false;
			command.Attach(trigger => triggered = true);
			if (mockResolver != null)
			{
				if (key)
					mockResolver.input.SetKeyboardState(Key.A, State.Pressed);
				else
					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Releasing);

				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.IsTrue(triggered);
				CheckTriggers(command, key);
			}
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
			if(key)
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

		[IntegrationTest]
		public void Run(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				command.Add(new KeyTrigger(Key.Y, State.Releasing));
				command.Run(input);
			});
		}

		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				command.Add(new KeyTrigger(Key.Y, State.Released));
				command.Run(input);
			});
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			Ellipse ellipse = null;
			var currentPosition = new Point(0.1f, 0.1f);
			Start(resolver, (EntitySystem entitySystem, InputCommands input) =>
			{
				ellipse = new Ellipse(new Rectangle(Point.Zero, new Size(0.1f, 0.1f)), Color.Red);
				entitySystem.Add(ellipse);
				input.Add(Key.A, State.Pressing, () => currentPosition = new Point(0.6f, 0.5f));
				input.Add(Key.A, State.Released, () => currentPosition = new Point(0.1f, 0.1f));
			}, () => ellipse.Center = currentPosition);
		}
	}
}