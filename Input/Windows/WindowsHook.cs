using System;

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
			int threadId = NativeMethods.GetCurrentThreadId();
			nativeCallbackLifetimeInstance = MessageCallback;
			hookHandle = NativeMethods.SetWindowsHookEx(hookType, nativeCallbackLifetimeInstance, 
				IntPtr.Zero, threadId);
		}

		private IntPtr hookHandle;
		private readonly NativeMethods.HookProc nativeCallbackLifetimeInstance;

		public virtual void Dispose()
		{
			NativeMethods.UnhookWindowsHookEx(hookHandle);
			hookHandle = IntPtr.Zero;
		}

		//ncrunch: no coverage start
		private int MessageCallback(int messageCode, IntPtr wParam, IntPtr lParam)
		{
			if (messageCode == HcAction || messageCode == WMTouch)
				HandleProcMessage(wParam, lParam, messageCode);
			return NativeMethods.CallNextHookEx(hookHandle, messageCode, wParam, lParam);
		}
		//ncrunch: no coverage end

		private const int HcAction = 0;
		internal const int WMTouch = 0x0240;

		protected internal abstract void HandleProcMessage(IntPtr wParam, IntPtr lParam, int msg);

		internal const int KeyboardHookId = 2;
		internal const int MouseHookId = 7;
	}
}