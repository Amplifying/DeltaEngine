using System;
using System.Collections.Generic;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InputCommandsTests : TestStarter
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
				input.Add(command);
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
				input.AddMouseHover(mouse => { });
				input.Add(touch => { });
				Assert.AreEqual(7, input.Count);
			});
		}

		[IntegrationTest]
		public void StoreAndRemoveCommands(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				List<Command> commands = CreateAndStoreCommands(input);
				Assert.AreEqual(9, input.Count);
				input.Remove(commands[2]);
				input.Remove(commands[4]);
				input.Remove(commands[7]);
				Assert.AreEqual(6, input.Count);
			});
		}

		private static List<Command> CreateAndStoreCommands(InputCommands input)
		{
			return new List<Command>
			{
				input.Add(Key.A, () => { }),
				input.Add(Key.A, State.Released, () => { }),
				input.Add(MouseButton.Left, mouse => { }),
				input.Add(MouseButton.Left, State.Pressing, mouse => { }),
				input.AddMouseMovement(mouse => { }),
				input.AddMouseHover(mouse => { }),
				input.Add(touch => { }),
				input.Add(State.Pressing, touch => { }),
				input.Add(GamePadButton.A, State.Pressing, () => { })
			};
		}

		[Test]
		public void AddTouchCallback()
		{
			var input = new InputCommands(null, null, null);
			input.Add(touch => { });
			Assert.AreEqual(1, input.Count);
		}

		[IntegrationTest]
		public void Clear(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				input.Add(Key.Space, State.Pressing, () => { });
				input.Add(Key.Escape, () => { });
				input.Add(MouseButton.Left, State.Pressing, mouse => { });
				input.Clear();
				Assert.AreEqual(0, input.Count);
			});
		}

		[VisualTest]
		public void QuitWithEscape(Type resolver)
		{
			Start(resolver,
				(InputCommands input, Window window) => input.Add(Key.Escape, window.Dispose));
		}
	}
}