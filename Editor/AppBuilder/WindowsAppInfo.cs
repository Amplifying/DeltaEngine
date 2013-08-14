using System;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.AppBuilder
{
	public class WindowsAppInfo : AppInfo
	{
		public WindowsAppInfo(string fullAppDataFilePath, Guid appGuid)
			: base(fullAppDataFilePath, appGuid, PlatformName.Windows) { }

		protected override Device[] GetAvailableDevices()
		{
			return new Device[] { new WindowsDevice() };
		}
	}
}