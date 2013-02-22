using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Touch structure of the native windows touch events containing all state information.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct NativeTouchInput
	{
		public int X;
		public int Y;
		private readonly IntPtr sourcePointer;
		public int Id;
		public int Flags;
		private readonly int mask;
		private readonly int timeStamp;
		private readonly IntPtr extraInfo;
		private readonly int contactX;
		private readonly int contactY;
	}
}