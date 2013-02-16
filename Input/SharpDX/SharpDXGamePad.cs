using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using XInput = SharpDX.XInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Native implementation of the GamePad interface using XInput.
	/// </summary>
	public class SharpDXGamePad : GamePad
	{
		public SharpDXGamePad()
		{
			controller = new XInput.Controller(XInput.UserIndex.One);
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.NotPressed;
		}

		private XInput.Controller controller;
		private readonly State[] states;

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

		public bool IsAvailable { get; private set; }

		public void Run()
		{
			XInput.State state = controller.GetState();
			leftThumbStick.X = NormalizeShortToFloat(state.Gamepad.LeftThumbX);
			leftThumbStick.Y = NormalizeShortToFloat(state.Gamepad.LeftThumbY);
			rightThumbStick.X = NormalizeShortToFloat(state.Gamepad.RightThumbX);
			rightThumbStick.Y = NormalizeShortToFloat(state.Gamepad.RightThumbY);
			leftTrigger = NormalizeByteToFloat(state.Gamepad.LeftTrigger);
			rightTrigger = NormalizeByteToFloat(state.Gamepad.RightTrigger);
			UpdateAllButtons((int)state.Gamepad.Buttons);
		}

		private Point leftThumbStick;
		private Point rightThumbStick;
		private float leftTrigger;
		private float rightTrigger;

		private void UpdateAllButtons(int bitfield)
		{
			Array buttons = XInputGamePadButton.A.GetEnumValues();
			foreach (XInputGamePadButton button in buttons)
				UpdateButton(bitfield, button);
		}

		private void UpdateButton(int bitfield, XInputGamePadButton button)
		{
			int buttonIndex = (int)ConvertButtonEnum(button);
			states[buttonIndex] = IsXInputButtonPressed(bitfield, button)
				? stateHelper.UpdateAndGetStateNowPressed(states[buttonIndex])
				: stateHelper.UpdateAndGetStateNowReleased(states[buttonIndex]);
		}

		private GamePadButton ConvertButtonEnum(XInputGamePadButton button)
		{
			switch (button)
			{
			case XInputGamePadButton.DPadUp:
				return GamePadButton.Up;
			case XInputGamePadButton.DPadDown:
				return GamePadButton.Down;
			case XInputGamePadButton.DPadLeft:
				return GamePadButton.Left;
			case XInputGamePadButton.DPadRight:
				return GamePadButton.Right;
			case XInputGamePadButton.Start:
				return GamePadButton.Start;
			case XInputGamePadButton.Back:
				return GamePadButton.Back;
			case XInputGamePadButton.LeftThumb:
				return GamePadButton.LeftStick;
			case XInputGamePadButton.RightThumb:
				return GamePadButton.RightStick;
			case XInputGamePadButton.LeftShoulder:
				return GamePadButton.LeftShoulder;
			case XInputGamePadButton.RightShoulder:
				return GamePadButton.RightShoulder;
			case XInputGamePadButton.A:
				return GamePadButton.A;
			case XInputGamePadButton.B:
				return GamePadButton.B;
			case XInputGamePadButton.X:
				return GamePadButton.X;
			case XInputGamePadButton.Y:
				return GamePadButton.Y;
			}
			return (GamePadButton)(-1);
		}

		private bool IsXInputButtonPressed(int bitfield, XInputGamePadButton button)
		{
			return (bitfield & (int)button) != 0;
		}

		private readonly InputStateHelper stateHelper = new InputStateHelper();

		private float NormalizeShortToFloat(short value)
		{
			return value * (1f / short.MaxValue);
		}

		private float NormalizeByteToFloat(byte value)
		{
			return value * (1f / byte.MaxValue);
		}

		public void Dispose()
		{
			controller = null;
		}
	}
}