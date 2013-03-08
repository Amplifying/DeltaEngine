using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Triggers;
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
				Assert.AreEqual(1, command.attachedTriggers.Count);
				command.Remove(trigger);
				Assert.AreEqual(0, command.attachedTriggers.Count);
			});
		}

		[IntegrationTest]
		public void SimulateMouseClick(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				input.Add(command);
				command.Add(new MouseButtonTrigger(MouseButton.Left, State.Releasing));
				bool triggered = false;
				command.Attach(trigger => triggered = true);
				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Releasing, Point.Zero);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(triggered);
				}
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
				var command = new Command();
				input.Add(command);
				command.Add(new KeyTrigger(Key.A, State.Pressed));
				bool triggered = false;
				command.Attach(trigger => triggered = true);
				if (testResolver != null)
				{
					testResolver.SetKeyboardState(Key.A, State.Pressed);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.IsTrue(triggered);
				}
			});
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
				var command = new TestResolver().Resolve<Command>();
				command.Add(new KeyTrigger(Key.Y, State.Released));
				command.Run(input);
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