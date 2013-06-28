using System.Collections.Generic;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InputCommandsTests : TestWithMocksOrVisually
	{
		[Test]
		public void CountCommandsForSimpleCommand()
		{
			Assert.AreEqual(0, Input.Count);
			Input.Add(Key.Space, key => { });
			Assert.AreEqual(1, Input.Count);
		}

		[Test]
		public void AddRemoveAndCountCommand()
		{
			var command = new Command();
			Input.Add(command);
			Input.Add(delegate { });
			Assert.AreEqual(2, Input.Count);
			Input.Add(MouseButton.Left, State.Pressed, delegate(Mouse mouse) { });
			Assert.AreEqual(3, Input.Count);
			Input.Remove(command);
			Assert.AreEqual(2, Input.Count);
			Input.Add(MouseButton.Middle, delegate { });
			Assert.AreEqual(3, Input.Count);
		}

		[Test]
		public void AddDifferentCommands()
		{
			Input.Add(Key.Space, State.Pressing, key => { });
			Input.Add(Key.Escape, key => { });
			Input.Add(MouseButton.Left, State.Pressing, mouse => { });
			Input.Add(MouseButton.Middle, delegate { });
			Input.AddMouseMovement(mouse => { });
			Input.AddMouseHover(mouse => { });
			Input.Add(touch => { });
			Assert.AreEqual(7, Input.Count);
		}

		[Test]
		public void StoreAndRemoveCommands()
		{
			List<Command> commands = CreateAndStoreCommands();
			Assert.AreEqual(9, Input.Count);
			Input.Remove(commands[2]);
			Input.Remove(commands[4]);
			Input.Remove(commands[7]);
			Assert.AreEqual(6, Input.Count);
		}

		private List<Command> CreateAndStoreCommands()
		{
			return new List<Command>
			{
				Input.Add(Key.A, key => { }),
				Input.Add(Key.A, State.Released, key => { }),
				Input.Add(MouseButton.Left, mouse => { }),
				Input.Add(MouseButton.Left, State.Pressing, mouse => { }),
				Input.AddMouseMovement(mouse => { }),
				Input.AddMouseHover(mouse => { }),
				Input.Add(touch => { }),
				Input.Add(State.Pressing, touch => { }),
				Input.Add(GamePadButton.A, State.Pressing, () => { })
			};
		}

		[Test]
		public void AddTouchCallback()
		{
			var input = new InputCommands(null, null, null, null);
			input.Add(touch => { });
			Assert.AreEqual(1, input.Count);
		}

		[Test]
		public void Clear()
		{
			Input.Add(Key.Space, State.Pressing, key => { });
			Input.Add(Key.Escape, key => { });
			Input.Add(MouseButton.Left, State.Pressing, mouse => { });
			Input.Clear();
			Assert.AreEqual(0, Input.Count);
		}

		[Test]
		public void QuitWithEscape()
		{
			Input.Add(Key.Escape, key => Resolve<MockResolver>().Window.Dispose());
		}
	}
}