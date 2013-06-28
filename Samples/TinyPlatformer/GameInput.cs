using System;
using DeltaEngine.Input;

namespace TinyPlatformer
{
	public class GameInput
	{
		/// <summary>
		/// Binding for the keyboard calls
		/// </summary>
		public GameInput(InputCommands inputCommands)
		{
			this.inputCommands = inputCommands;
			SetControls();
		}

		private readonly InputCommands inputCommands;

		public void SetControls()
		{
			inputCommands.Clear();
			inputCommands.Add(Key.CursorLeft, State.Pressed, key => LeftDown());
			inputCommands.Add(Key.CursorRight, State.Pressed, key => RightDown());
			inputCommands.Add(Key.Space, State.Pressed, key => JumpDown());
			inputCommands.Add(Key.CursorLeft, State.Released, key => LeftUp());
			inputCommands.Add(Key.CursorRight, State.Released, key => RightUp());
			inputCommands.Add(Key.Space, State.Released, key => JumpUp());		
		}

		public event Action LeftDown, RightDown, JumpDown, LeftUp, RightUp, JumpUp;
	}
}