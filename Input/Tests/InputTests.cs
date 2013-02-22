using System;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InputConfigurationTests : TestStarter
	{
		[IntegrationTest]
		public void CountCommandsForSimpleCommand(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				Assert.AreEqual(0, input.Count);
				input.Add(Key.Space, () => { });
				Assert.AreEqual(1, input.Count);
			});
		}

		[IntegrationTest]
		public void AddRemoveAndCountCommand(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var command = new Command();
				Assert.AreEqual(0, input.Count);
				input.Add(command);
				Assert.AreEqual(1, input.Count);
				input.Add(delegate { });
				Assert.AreEqual(2, input.Count);
				input.Add(MouseButton.Left, State.Pressed, delegate(Mouse mouse) { });
				Assert.AreEqual(3, input.Count);
				input.Remove(command);
				Assert.AreEqual(2, input.Count);
				input.Add(MouseButton.Middle, delegate(Mouse mouse) { });
				Assert.AreEqual(3, input.Count);
			});
		}

		[IntegrationTest]
		public void AddDifferentCommands(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				input.Add(Key.Space, State.Pressing, () => { });
				input.Add(Key.Escape, () => { });
				input.Add(MouseButton.Left, State.Pressing, mouse => { });
				input.Add(MouseButton.Middle, delegate(Mouse mouse) { });
				input.AddMouseMovement(mouse => { });
				input.Add(touch => { });
				Assert.AreEqual(6, input.Count);
			});
		}

		[Test]
		public void AddTouchCallback()
		{
			var input = new InputCommands(null, null, null, null);
			input.Add((Touch touch) => {});
			Assert.AreEqual(1, input.Count);
		}

		[VisualTest]
		public void QuitWithEscape(Type resolver)
		{
			Start(resolver, (InputCommands input, Window window) => input.Add(Key.Escape, window.Dispose));
		}
	}
}