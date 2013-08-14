using System;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.AppBuilder
{
	public class AndroidAppInfo : AppInfo
	{
		public AndroidAppInfo(string fullAppDataFilePath, Guid appGuid)
			: base(fullAppDataFilePath, appGuid, PlatformName.Android) {}

		protected override Device[] GetAvailableDevices()
		{
			return new AndroidDeviceFinder().GetAvailableDevices();
		}
	}
}