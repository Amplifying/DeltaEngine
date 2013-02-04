using System;
using System.Runtime.InteropServices;

#pragma warning disable 612
#pragma warning disable 618

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// This class is a wrapper around the Windows Message Hook pipeline, which is basically
	/// the same as Application.DoEvents()
	/// </summary>
	public abstract class WindowsHook : IDisposable
	{
		protected WindowsHook(int hookType)
		{
			int threadId = AppDomain.GetCurrentThreadId();
			nativeCallbackLifetimeInstance = MessageCallback;
			hookHandle = SetWindowsHookEx(hookType, nativeCallbackLifetimeInstance, IntPtr.Zero,
				threadId);
		}

		//ncrunch: no coverage start
		~WindowsHook()
		{
			Dispose();
		}
		//ncrunch: no coverage end

		private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

		private IntPtr hookHandle;
		private readonly HookProc nativeCallbackLifetimeInstance;

		public virtual void Dispose()
		{
			UnhookWindowsHookEx(hookHandle);
			hookHandle = IntPtr.Zero;
		}

		//ncrunch: no coverage start
		private int MessageCallback(int messageCode, IntPtr wParam, IntPtr lParam)
		{
			if (messageCode == HcAction || messageCode == WMTouch)
				HandleProcMessage(wParam, lParam, messageCode);
			return CallNextHookEx(hookHandle, messageCode, wParam, lParam);
		}
		//ncrunch: no coverage end

		private const int HcAction = 0;
		protected const int WMTouch = 0x0240;

		protected internal abstract void HandleProcMessage(IntPtr wParam, IntPtr lParam, int msg);

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(int code, HookProc func, IntPtr hInstance,
			int threadID);

		[DllImport("user32.dll")]
		private static extern int UnhookWindowsHookEx(IntPtr hook);

		[DllImport("user32.dll")]
		private static extern int CallNextHookEx(IntPtr hook, int code, IntPtr wParam, IntPtr lParam);

		protected const int KeyboardHookId = 2;
		protected const int MouseHookId = 7;
	}
}