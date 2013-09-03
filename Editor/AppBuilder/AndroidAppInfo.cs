using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public class AndroidAppInfo : AppInfo
	{
		public AndroidAppInfo(string fullAppDataFilePath, Guid appGuid, DateTime buildDate)
			: base(fullAppDataFilePath, appGuid, PlatformName.Android, buildDate) { }

		//protected override Device[] GetAvailableDevices()
		//{
		//	return new AndroidDeviceFinder().GetAvailableDevices();
		//}
	}
}