using System;
using DeltaEngine.Datatypes;
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
			Start(resolver, (Input input) =>
			{
				Assert.AreEqual(0, input.NumberOfCommands);
				input.Add(Key.Space, () => { });
				Assert.AreEqual(1, input.NumberOfCommands);
			});
		}

		[IntegrationTest]
		public void AddRemoveAndCountCommand(Type resolver)
		{
			Start(resolver, (Input input) =>
			{
				var command = new Command();
				Assert.AreEqual(0, input.NumberOfCommands);
				input.Add(command);
				Assert.AreEqual(1, input.NumberOfCommands);
				input.Add(delegate(Touch touch) { });
				Assert.AreEqual(2, input.NumberOfCommands);
				input.Add(MouseButton.Left, State.Pressed, delegate(Mouse mouse) { });
				Assert.AreEqual(3, input.NumberOfCommands);
				input.Remove(command);
				Assert.AreEqual(2, input.NumberOfCommands);
				input.Add(MouseButton.Middle, delegate(Mouse mouse) { });
				Assert.AreEqual(3, input.NumberOfCommands);
			});
		}

		[VisualTest]
		public void QuitWithEscape(Type resolver)
		{
			Start(resolver, (Input input, Window window) => input.Add(Key.Escape, window.Dispose));
		}
	}
}