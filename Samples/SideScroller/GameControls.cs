using System;
using DeltaEngine.Input;

namespace SideScroller
{
	public class GameControls
	{
		public GameControls(InputCommands inputCommands)
		{
			this.inputCommands = inputCommands;
			SetControlsToState();
		}

		private readonly InputCommands inputCommands;

		public void SetControlsToState()
		{
			inputCommands.Clear();

			AddAscensionControls();
			AddSinkingControls();
			AddFireingControls();
			AddAccelerationControls();
			AddSlowingDownControls();
		}

		private void AddAscensionControls()
		{
			inputCommands.Add(Key.W, State.Pressed, key => Ascend());
			inputCommands.Add(Key.W, State.Pressing, key => Ascend());
			inputCommands.Add(Key.CursorUp, State.Pressed, key => Ascend());
			inputCommands.Add(Key.CursorUp, State.Pressing, key => Ascend());
			inputCommands.Add(Key.W, State.Releasing, key => VerticalStop());
			inputCommands.Add(Key.CursorUp, State.Releasing, key => VerticalStop());
		}

		private void AddSinkingControls()
		{
			inputCommands.Add(Key.S, State.Pressed, key => Sink());
			inputCommands.Add(Key.S, State.Pressing, key => Sink());
			inputCommands.Add(Key.CursorDown, State.Pressed, key => Sink());
			inputCommands.Add(Key.CursorDown, State.Pressing, key => Sink());
			inputCommands.Add(Key.S, State.Releasing, key => VerticalStop());
			inputCommands.Add(Key.CursorDown, State.Releasing, key => VerticalStop());
		}

		private void AddFireingControls()
		{
			inputCommands.Add(Key.Space, State.Pressing, key => Fire());
			inputCommands.Add(Key.Space, State.Releasing, key => HoldFire());
		}

		private void AddAccelerationControls()
		{
			inputCommands.Add(Key.D, State.Pressed, key => Accelerate());
			inputCommands.Add(Key.D, State.Pressing, key => Accelerate());
			inputCommands.Add(Key.CursorRight, State.Pressed, key => Accelerate());
			inputCommands.Add(Key.CursorRight, State.Pressing, key => Accelerate());
		}

		private void AddSlowingDownControls()
		{
			inputCommands.Add(Key.A, State.Pressed, key => SlowDown());
			inputCommands.Add(Key.A, State.Pressing, key => SlowDown());
			inputCommands.Add(Key.CursorLeft, State.Pressed, key => SlowDown());
			inputCommands.Add(Key.CursorLeft, State.Pressing, key => SlowDown());
		}

		public event Action Ascend , Sink , VerticalStop , Accelerate , SlowDown , Fire , HoldFire;
	}
}