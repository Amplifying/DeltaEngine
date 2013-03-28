using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class CommandTests : TestStarter
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
				if (testResolver != null)
				{
					testResolver.SetMousePosition(Point.Half);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					testResolver.SetMousePosition(Point.One);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(command.TriggerFired);
				}
			});
		}

		[IntegrationTest]
		public void SimulateKeyPress(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				SimulateKeyOrMousePress(input, true);
			});
		}

		private void SimulateKeyOrMousePress(InputCommands input, bool key)
		{
			var command = new Command();
			InputIsKeyOrMouse(input, key, command);
			bool triggered = false;
			command.Attach(trigger => triggered = true);
			if (testResolver != null)
			{
				if (key)
					testResolver.SetKeyboardState(Key.A, State.Pressed);
				else
					testResolver.SetMouseButtonState(MouseButton.Left, State.Releasing);

				testResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.IsTrue(triggered);
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

		[IntegrationTest]
		public void Run(Type resolver)
		{
			Start(resolver, (InputCommands input, Time time) =>
			{
				var command = new Command();
				command.Add(new KeyTrigger(Key.Y, State.Releasing));
				command.Run(input, time);
			});
		}

		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new TestResolver().Resolve<Command>();
				command.Add(new KeyTrigger(Key.Y, State.Released));
				command.Run(input, null);
			});
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			Rect rectangle = null;
			var currentPosition = new Point(0.1f, 0.1f);

			Start(resolver, (Renderer renderer, InputCommands input) =>
			{
				rectangle = new Rect(Point.Zero, new Size(0.1f, 0.1f), Color.Red);
				renderer.Add(rectangle);
				input.Add(Key.A, State.Pressing, () => currentPosition = new Point(0.6f, 0.5f));
				input.Add(Key.A, State.Released, () => currentPosition = new Point(0.1f, 0.1f));
			}, () => rectangle.DrawArea.TopLeft = currentPosition);
		}
	}
}