using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using Pencil.Gaming;

namespace DeltaEngine.Input.GLFW
{
	/// <summary>
	/// GLFW implementation of a GamePad.
	/// Not all features of the Xbox GamePad are available.
	/// </summary>
	public class GLFWGamePad : GamePad
	{
		public GLFWGamePad()
			: this(GamePadNumber.Any) {}

		public GLFWGamePad(GamePadNumber number)
			: base(number)
		{
			Glfw.Init();
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.Released;
		}

		private readonly State[] states;

		public override void Vibrate(float strength) {}

		public override void Run()
		{
			axisValues = new float[6];
			Glfw.GetJoystickPos(GetJoystickByNumber(), axisValues, axisValues.Length);
			byte[] buttons = new byte[10];
			Glfw.GetJoystickButtons(GetJoystickByNumber(), buttons, buttons.Length);
			UpdateAllButtons(buttons);
		}

		private float[] axisValues;

		private void UpdateAllButtons(byte[] buttons)
		{
			UpdateNormalButtons(buttons);
			UpdateStickAndShoulderButtons(buttons);
		}

		private void UpdateNormalButtons(byte[] buttons)
		{
			UpdateButton(buttons[0], GamePadButton.A);
			UpdateButton(buttons[1], GamePadButton.B);
			UpdateButton(buttons[2], GamePadButton.X);
			UpdateButton(buttons[3], GamePadButton.Y);
			UpdateButton(buttons[6], GamePadButton.Back);
			UpdateButton(buttons[7], GamePadButton.Start);
		}

		private void UpdateStickAndShoulderButtons(byte[] buttons)
		{
			UpdateButton(buttons[4], GamePadButton.LeftShoulder);
			UpdateButton(buttons[8], GamePadButton.LeftStick);
			UpdateButton(buttons[5], GamePadButton.RightShoulder);
			UpdateButton(buttons[9], GamePadButton.RightStick);
		}

		private void UpdateButton(byte newState, GamePadButton button)
		{
			var buttonIndex = (int)button;
			states[buttonIndex] = states[buttonIndex].UpdateOnNativePressing(newState == 1);
		}

		public override void Dispose() {}
		public override bool IsAvailable
		{
			get { return GetPresence(GetJoystickByNumber()); }
		}

		private bool GetPresence(Joystick joystick)
		{
			return Glfw.GetJoystickParam(joystick, JoystickParam.Present) != 0;
		}

		private Joystick GetJoystickByNumber()
		{
			if (Number == GamePadNumber.Any)
				return GetAnyJoystick();
			if (Number == GamePadNumber.Two)
				return Joystick.Joystick2;
			if (Number == GamePadNumber.Three)
				return Joystick.Joystick3;
			return Number == GamePadNumber.Four ? Joystick.Joystick4 : Joystick.Joystick1;
		}

		private Joystick GetAnyJoystick()
		{
			if (GetPresence(Joystick.Joystick2))
				return Joystick.Joystick2;
			if (GetPresence(Joystick.Joystick3))
				return Joystick.Joystick3;
			return GetPresence(Joystick.Joystick4) ? Joystick.Joystick4 : Joystick.Joystick1;
		}

		public override Point GetLeftThumbStick()
		{
			return new Point(axisValues[0], axisValues[1]);
		}

		public override Point GetRightThumbStick()
		{
			return new Point(axisValues[4], -axisValues[3]);
		}

		public override float GetLeftTrigger()
		{
			return axisValues[2];
		}

		public override float GetRightTrigger()
		{
			return axisValues[2];
		}

		public override State GetButtonState(GamePadButton button)
		{
			return states[(int)button];
		}
	}
}