using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using XInput = SharpDX.XInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Native implementation of the GamePad interface using XInput.
	/// </summary>
	public class SharpDXGamePad : GamePad
	{
		public SharpDXGamePad() : this(GamePadNumber.Any) {}

		public SharpDXGamePad(GamePadNumber number)
			: base(number)
		{
			controller = new XInput.Controller(GetUserIndexFromNumber());
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.Released;
		}

		private XInput.Controller controller;
		private readonly State[] states;

		private XInput.UserIndex GetUserIndexFromNumber()
		{
			if (Number == GamePadNumber.Any)
				return XInput.UserIndex.Any;
			if (Number == GamePadNumber.Two)
				return XInput.UserIndex.Two;
			if (Number == GamePadNumber.Three)
				return XInput.UserIndex.Three;
			return Number == GamePadNumber.Four ? XInput.UserIndex.Four : XInput.UserIndex.One;
		}

		public override void Dispose()
		{
			controller = null;
		}

		public override bool IsAvailable
		{
			get { return controller.IsConnected; }
		}

		public override void Vibrate(float strength)
		{
			short motorSpeed = (short)(strength * short.MaxValue);
			controller.SetVibration(new XInput.Vibration
			{ LeftMotorSpeed = motorSpeed, RightMotorSpeed = motorSpeed });
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

		private Point leftThumbStick;
		private Point rightThumbStick;
		private float leftTrigger;
		private float rightTrigger;

		private static float NormalizeShortToFloat(short value)
		{
			return value * (1f / short.MaxValue);
		}

		private static float NormalizeByteToFloat(byte value)
		{
			return value * (1f / byte.MaxValue);
		}

		private void UpdateAllButtons(int bitfield)
		{
			Array buttons = XInputGamePadButton.A.GetEnumValues();
			foreach (XInputGamePadButton button in buttons)
				UpdateButton(ConvertButtonEnum(button), IsXInputButtonPressed(bitfield, button));
		}

		private void UpdateButton(GamePadButton button, bool nowPressed)
		{
			var buttonIndex = (int)button;
			states[buttonIndex] = states[buttonIndex].UpdateOnNativePressing(nowPressed);
		}

		private static GamePadButton ConvertButtonEnum(XInputGamePadButton button)
		{
			if (IsPadDirectionButton(button))
				return ConvertPadDirections(button);
			if (IsPadButton(button))
				return ConvertButtons(button);
			if (IsPadStick(button))
				return ConvertSticks(button);
			if (IsPadShoulder(button))
				return ConvertShoulders(button);
			return ConvertStartOrBack(button);
		}

		private static bool IsPadDirectionButton(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.DPadUp || button == XInputGamePadButton.DPadDown ||
				button == XInputGamePadButton.DPadLeft || button == XInputGamePadButton.DPadRight;
		}

		private static GamePadButton ConvertPadDirections(XInputGamePadButton button)
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
			}
			return (GamePadButton)(-1);
		}

		private static bool IsPadButton(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.A || button == XInputGamePadButton.B ||
				button == XInputGamePadButton.X || button == XInputGamePadButton.Y;
		}

		private static GamePadButton ConvertButtons(XInputGamePadButton button)
		{
			switch (button)
			{
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

		private static bool IsPadStick(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.LeftThumb || button == XInputGamePadButton.RightThumb;
		}

		private static GamePadButton ConvertSticks(XInputGamePadButton button)
		{
			switch (button)
			{
				case XInputGamePadButton.LeftThumb:
					return GamePadButton.LeftStick;
				case XInputGamePadButton.RightThumb:
					return GamePadButton.RightStick;
			}
			return (GamePadButton)(-1);
		}

		private static bool IsPadShoulder(XInputGamePadButton button)
		{
			return button == XInputGamePadButton.LeftShoulder ||
				button == XInputGamePadButton.RightShoulder;
		}

		private static GamePadButton ConvertShoulders(XInputGamePadButton button)
		{
			switch (button)
			{
				case XInputGamePadButton.LeftShoulder:
					return GamePadButton.LeftShoulder;
				case XInputGamePadButton.RightShoulder:
					return GamePadButton.RightShoulder;
			}
			return (GamePadButton)(-1);
		}

		private static GamePadButton ConvertStartOrBack(XInputGamePadButton button)
		{
			if (button == XInputGamePadButton.Start)
				return GamePadButton.Start;
			if (button == XInputGamePadButton.Back)
				return GamePadButton.Back;
			return (GamePadButton)(-1);
		}

		private static bool IsXInputButtonPressed(int bitfield, XInputGamePadButton button)
		{
			return (bitfield & (int)button) != 0;
		}

		public override Point GetLeftThumbStick()
		{
			return leftThumbStick;
		}

		public override Point GetRightThumbStick()
		{
			return rightThumbStick;
		}

		public override float GetLeftTrigger()
		{
			return leftTrigger;
		}

		public override float GetRightTrigger()
		{
			return rightTrigger;
		}

		public override State GetButtonState(GamePadButton button)
		{
			return states[(int)button];
		}
	}
}