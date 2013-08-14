using System;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.AppBuilder
{
	public class WP7AppInfo : AppInfo
	{
		public WP7AppInfo(string fullAppDataFilePath, Guid appGuid)
			: base(fullAppDataFilePath, appGuid, PlatformName.WindowsPhone7) { }

		protected override Device[] GetAvailableDevices()
		{
			return new WP7DeviceFinder().GetAvailableDevices();
		}
	}
}