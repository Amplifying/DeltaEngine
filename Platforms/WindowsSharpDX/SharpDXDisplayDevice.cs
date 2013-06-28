using System.Runtime.InteropServices;

namespace DeltaEngine.Platforms
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal class SharpDXDisplayDevice
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string deviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		// ReSharper disable UnassignedField.Global
		internal string deviceString;
		// ReSharper restore UnassignedField.Global

		internal uint stateFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string deviceID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string deviceKey;
	}
}