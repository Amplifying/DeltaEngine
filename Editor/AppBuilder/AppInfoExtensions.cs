using System;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.AppBuilder.Web;
using DeltaEngine.Editor.AppBuilder.WindowsPhone7;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public static class AppInfoExtensions
	{
		//this should be a factory with a plugin system
		public static AppInfo CreateAppInfo(string appFilePath, PlatformName platform, Guid appGuid,
			DateTime buildDate)
		{
			switch (platform)
			{
				case PlatformName.Windows:
					return new WindowsAppInfo(appFilePath, appGuid, buildDate);
				case PlatformName.WindowsPhone7:
					return new WP7AppInfo(appFilePath, appGuid, buildDate);
				case PlatformName.Android:
					return new AndroidAppInfo(appFilePath, appGuid, buildDate);
				case PlatformName.Web:
					return new WebAppInfo(appFilePath, appGuid, buildDate);
				default:
					throw new UnsupportedPlatfromForAppInfo(appFilePath, platform);
			}
		}

		private class UnsupportedPlatfromForAppInfo : Exception
		{
			public UnsupportedPlatfromForAppInfo(string appFilePath, PlatformName platform)
				: base(platform + ": " + appFilePath) { }
		}
	}
}
