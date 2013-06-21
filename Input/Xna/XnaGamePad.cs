using DeltaEngine.Core;
using Microsoft.Xna.Framework;
using Point = DeltaEngine.Datatypes.Point;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native implementation of the GamePad interface using Xna and PlayerIndex.One
	/// </summary>
	public class XnaGamePad : GamePad
	{
		public XnaGamePad()
		{
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.Released;
		}
		private readonly State[] states;

		public void Run()
		{
			XnaInput.GamePadState state = XnaInput.GamePad.GetState(PlayerIndex.One);
			IsAvailable = state.IsConnected;
			if (IsAvailable)
				UpdateValuesFromState(state);
		}

		public bool IsAvailable { get; private set; }

		private void UpdateValuesFromState(XnaInput.GamePadState state)
		{
			leftThumbStick.X = state.ThumbSticks.Left.X;
			leftThumbStick.Y = state.ThumbSticks.Left.Y;
			rightThumbStick.X = state.ThumbSticks.Right.X;
			rightThumbStick.Y = state.ThumbSticks.Right.Y;
			leftTrigger = state.Triggers.Left;
			rightTrigger = state.Triggers.Right;
			UpdateAllButtons(state);
		}

		private Point leftThumbStick;
		private Point rightThumbStick;
		private float leftTrigger;
		private float rightTrigger;

		private void UpdateAllButtons(XnaInput.GamePadState state)
		{
			UpdateNormalButtons(state);
			UpdateStickAndShoulderButtons(state);
			UpdateDPadButtons(state);
		}

		private void UpdateNormalButtons(XnaInput.GamePadState state)
		{
			UpdateButton(state.Buttons.A, GamePadButton.A);
			UpdateButton(state.Buttons.B, GamePadButton.B);
			UpdateButton(state.Buttons.X, GamePadButton.X);
			UpdateButton(state.Buttons.Y, GamePadButton.Y);
			UpdateButton(state.Buttons.Back, GamePadButton.Back);
			UpdateButton(state.Buttons.Start, GamePadButton.Start);
			UpdateButton(state.Buttons.BigButton, GamePadButton.BigButton);
		}

		private void UpdateStickAndShoulderButtons(XnaInput.GamePadState state)
		{
			UpdateButton(state.Buttons.LeftShoulder, GamePadButton.LeftShoulder);
			UpdateButton(state.Buttons.LeftStick, GamePadButton.LeftStick);
			UpdateButton(state.Buttons.RightShoulder, GamePadButton.RightShoulder);
			UpdateButton(state.Buttons.RightStick, GamePadButton.RightStick);
		}

		private void UpdateDPadButtons(XnaInput.GamePadState state)
		{
			UpdateButton(state.DPad.Down, GamePadButton.Down);
			UpdateButton(state.DPad.Up, GamePadButton.Up);
			UpdateButton(state.DPad.Left, GamePadButton.Left);
			UpdateButton(state.DPad.Right, GamePadButton.Right);
		}

		private void UpdateButton(XnaInput.ButtonState newState, GamePadButton button)
		{
			var buttonIndex = (int)button;
			states[buttonIndex] =
				states[buttonIndex].UpdateOnNativePressing(newState == XnaInput.ButtonState.Pressed);
		}

		public void Dispose()
		{
			IsAvailable = false;
		}

		public Point GetLeftThumbStick()
		{
			return leftThumbStick;
		}

		public Point GetRightThumbStick()
		{
			return rightThumbStick;
		}

		public float GetLeftTrigger()
		{
			return leftTrigger;
		}

		public float GetRightTrigger()
		{
			return rightTrigger;
		}

		public State GetButtonState(GamePadButton button)
		{
			return states[(int)button];
		}
	}
}