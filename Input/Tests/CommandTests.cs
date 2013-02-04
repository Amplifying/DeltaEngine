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
		[IntegrationTest]
		public void AddAndRemoveTrigger(Type resolver)
		{
			Start(resolver, (Input input) =>
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
			Start(resolver, (Input input) =>
			{
				var command = new MockCommand();
				command.Add(new MouseButtonTrigger(MouseButton.Left, State.Releasing));
				command.Simulate(MouseButton.Left);
				Assert.IsTrue(command.TriggerFired);
			});
		}

		[IntegrationTest]
		public void SimulateKeyPress(Type resolver)
		{
			Start(resolver, (Input input) =>
			{
				var command = new MockCommand();
				command.Add(new KeyTrigger(Key.A, State.Pressed));
				command.Simulate(Key.A);
				Assert.IsTrue(command.TriggerFired);
			});
		}

		[IntegrationTest]
		public void Run(Type resolver)
		{
			Start(resolver, (Input input) =>
			{
				var command = new MockCommand();
				command.Add(new KeyTrigger(Key.Y, State.Releasing));
				command.Run(input);
			});
		}

		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (Input input) =>
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
			//TODO: simplify
			var pressCommand = new Command();
			var releaseCommand = new Command();
			pressCommand.Callback += t => currentPosition = new Point(0.6f, 0.5f);
			releaseCommand.Callback += t => currentPosition = new Point(0.1f, 0.1f);
			var pressTrigger = new KeyTrigger(Key.A, State.Pressing);
			var releaseTrigger = new KeyTrigger(Key.A, State.Releasing);
			pressCommand.Add(pressTrigger);
			releaseCommand.Add(releaseTrigger);

			Start(resolver, (Renderer renderer, Input input) =>
			{
				rectangle = new ColoredRectangle(Point.Zero, new Size(0.1f, 0.1f), Color.Red);
				renderer.Add(rectangle);
				input.Add(pressCommand);
				input.Add(releaseCommand);
			}, () => rectangle.DrawArea.TopLeft = currentPosition);
		}
	}
}