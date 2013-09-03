using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public class WindowsAppInfo : AppInfo
	{
		public WindowsAppInfo(string fullAppDataFilePath, Guid appGuid, DateTime buildDate)
			: base(fullAppDataFilePath, appGuid, PlatformName.Windows, buildDate) { }

		//protected override Device[] GetAvailableDevices()
		//{
		//	return new Device[] { new WindowsDevice() };
		//}
	}
}