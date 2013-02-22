using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native hook on the windows messaging pipeline to grab touch input data.
	/// </summary>
	public class TouchHook : WindowsHook
	{
		public TouchHook(Window window)
			: base(WMTouch)
		{
			nativeTouches = new List<NativeTouchInput>();
			windowHandle = window.Handle;
			RegisterTouchWindow(windowHandle, 0);
			RegisterNativeTouchEvent(window);
		}

		internal readonly List<NativeTouchInput> nativeTouches;
		private readonly IntPtr windowHandle;

		private void RegisterNativeTouchEvent(Window window)
		{
			if(!(window is FormsWindow))
				return;

			var formsWindow = window as FormsWindow;
			formsWindow.NativeEvent += delegate(ref Message message)
			{
				HandleProcMessage(message.WParam, message.LParam, message.Msg);
			};
		}

		protected internal override void HandleProcMessage(IntPtr wParam, IntPtr lParam, int msg)
		{
			if(msg != WMTouch)
				return;
			int inputCount = wParam.ToInt32();
			NativeTouchInput[] newTouches = GetTouchDataFromHandle(inputCount, lParam);
			CloseTouchInputHandle(lParam);
			if (newTouches != null)
				nativeTouches.AddRange(newTouches);
		}

		internal NativeTouchInput[] GetTouchDataFromHandle(int inputCount, IntPtr handle)
		{
			var inputs = new NativeTouchInput[inputCount];
			bool isTouchProcessed = GetTouchInputInfo(handle, inputCount, inputs, NativeTouchByteSize);
			return isTouchProcessed == false ? null : inputs;
		}

		private static readonly int NativeTouchByteSize = Marshal.SizeOf(typeof(NativeTouchInput));

		public override void Dispose()
		{
			if (windowHandle != IntPtr.Zero)
				UnregisterTouchWindow(windowHandle);
			base.Dispose();
		}

		[DllImport("User32")]
		private static extern bool RegisterTouchWindow(IntPtr handle, UInt32 flags);

		[DllImport("User32")]
		private static extern bool UnregisterTouchWindow(IntPtr handle);

		[DllImport("user32")]
		private static extern void CloseTouchInputHandle(IntPtr lParam);

		[DllImport("user32")]
		private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs,
			[In, Out] NativeTouchInput[] pInputs, int cbSize);
	}
}