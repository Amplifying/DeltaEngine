using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder.WindowsPhone7
{
	public class WP7AppInfo : AppInfo
	{
		public WP7AppInfo(string fullAppDataFilePath, Guid appGuid, DateTime buildDate)
			: base(fullAppDataFilePath, appGuid, PlatformName.WindowsPhone7, buildDate) { }

		protected override Device[] GetAvailableDevices()
		{
			return new WP7DeviceFinder().GetAvailableDevices();
		}
	}
}