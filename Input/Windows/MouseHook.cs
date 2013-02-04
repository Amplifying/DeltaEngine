using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DeltaEngine.Core;
using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native hook on the windows messaging pipeline to grab mouse input data.
	/// </summary>
	internal class MouseHook : WindowsHook
	{
		internal MouseHook()
			: base(MouseHookId) {}

		public int ScrollWheelValue { get; private set; }

		internal State ProcessButtonQueue(State previousState, MouseButton button)
		{
			if (previousState == State.Released && currentlyPressed[(int)button] == false &&
				wasReleasedThisFrame[(int)button])
				return State.Pressing;

			wasReleasedThisFrame[(int)button] = false;
			return previousState.UpdateOnNativePressing(currentlyPressed[(int)button]);
		}

		private readonly bool[] currentlyPressed = new bool[MouseButton.Left.GetCount()];
		private readonly bool[] wasReleasedThisFrame = new bool[MouseButton.Left.GetCount()];

		protected internal override void HandleProcMessage(IntPtr wParam, IntPtr lParam, int msg)
		{
			int[] data = new int[6];
			Marshal.Copy(lParam, data, 0, 6);
			UpdateMouseButtonsAndWheel(wParam.ToInt32(), data[5]);
		}

		private void UpdateMouseButtonsAndWheel(int intParam, int mouseData)
		{
			if (intParam == WMMousewheel)
				ScrollWheelValue += mouseData / 120;
			else
				UpdateMouseButton(intParam, mouseData);
		}

		internal const int WMMousewheel = 0x020A;

		private void UpdateMouseButton(int intParam, int mouseData)
		{
			bool isPressed = MouseButtonStateHelper.IsPressed(intParam);
			MouseButton button = MouseButtonStateHelper.GetMessageButton(intParam, mouseData);
			currentlyPressed[(int)button] = isPressed;
			if (isPressed == false)
				wasReleasedThisFrame[(int)button] = true;
		}
	}
}