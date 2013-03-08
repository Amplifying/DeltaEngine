using System;
using DeltaEngine.Core;
using DeltaEngine.Input.Devices;
using XInput = SharpDX.XInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Native implementation of the GamePad interface using XInput.
	/// </summary>
	public class SharpDXGamePad : BaseGamePad
	{
		public SharpDXGamePad()
		{
			controller = new XInput.Controller(XInput.UserIndex.One);
		}

		private XInput.Controller controller;

		public override bool IsAvailable
		{
			get { return controller.IsConnected; }
		}

		public override void Run()
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

		private void UpdateAllButtons(int bitfield)
		{
			Array buttons = XInputGamePadButton.A.GetEnumValues();
			foreach (XInputGamePadButton button in buttons)
				UpdateButton(ConvertButtonEnum(button), IsXInputButtonPressed(bitfield, button));
		}

		private static GamePadButton ConvertButtonEnum(XInputGamePadButton button)
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

		private static bool IsXInputButtonPressed(int bitfield, XInputGamePadButton button)
		{
			return (bitfield & (int)button) != 0;
		}

		private static float NormalizeShortToFloat(short value)
		{
			return value * (1f / short.MaxValue);
		}

		private static float NormalizeByteToFloat(byte value)
		{
			return value * (1f / byte.MaxValue);
		}

		public override void Dispose()
		{
			controller = null;
		}
	}
}