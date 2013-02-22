using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Triggers;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class CommandTests : TestStarter
	{
		[Test]
		public void AddAndRemove()
		{
			var command = new Command();
			var trigger = new MouseButtonTrigger(MouseButton.Left, State.Released);
			command.Add(trigger);
			Assert.AreEqual(1, command.attachedTriggers.Count);
			command.Remove(trigger);
			Assert.AreEqual(0, command.attachedTriggers.Count);
		}

		[IntegrationTest]
		public void AddAndRemoveTrigger(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new MockCommand();
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
				var command = new MockCommand();
				command.Add(new MouseButtonTrigger(MouseButton.Left, State.Releasing));
				bool triggered = false;
				command.Attach(trigger => triggered = true);
				command.Simulate(MouseButton.Left);
				Assert.IsTrue(triggered);
			});
		}

		[IntegrationTest]
		public void SimulateMouseMovement(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new MockCommand();
				command.Add(new MouseMovementTrigger());
				command.SimulateMovement();
				Assert.IsTrue(command.TriggerFired);
			});
		}

		[IntegrationTest]
		public void SimulateKeyPress(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new MockCommand();
				command.Add(new KeyTrigger(Key.A, State.Pressed));
				bool triggered = false;
				command.Attach(trigger => triggered = true);
				command.Simulate(Key.A);
				Assert.IsTrue(triggered);
			});
		}

		[IntegrationTest]
		public void Run(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new MockCommand();
				command.Add(new KeyTrigger(Key.Y, State.Releasing));
				command.Run(input);
			});
		}

		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new MockCommand();
				command.Add(new KeyTrigger(Key.Y, State.Released));
				command.Run(input);
			});
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			ColoredRectangle rectangle = null;
			var currentPosition = new Point(0.1f, 0.1f);

			Start(resolver, (Renderer renderer, InputCommands input) =>
			{
				rectangle = new ColoredRectangle(Point.Zero, new Size(0.1f, 0.1f), Color.Red);
				renderer.Add(rectangle);
				input.Add(Key.A, State.Pressing, () => currentPosition = new Point(0.6f, 0.5f));
				input.Add(Key.A, State.Released, () => currentPosition = new Point(0.1f, 0.1f));
			}, () => rectangle.DrawArea.TopLeft = currentPosition);
		}
	}
}