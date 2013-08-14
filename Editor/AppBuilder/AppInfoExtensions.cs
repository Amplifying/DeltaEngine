using System;
using System.IO;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public static class AppInfoExtensions
	{
		public static AppInfo CreateAppInfo(string appFilePath, PlatformName platform, Guid appGuid)
		{
			switch (platform)
			{
				case PlatformName.Windows:
					return new WindowsAppInfo(appFilePath, appGuid);
				case PlatformName.WindowsPhone7:
					return new WP7AppInfo(appFilePath, appGuid);
				case PlatformName.Android:
					return new AndroidAppInfo(appFilePath, appGuid);
				default:
					throw new UnsupportedPlatfromForAppInfo(appFilePath, platform);
			}
		}

		private class UnsupportedPlatfromForAppInfo : Exception
		{
			public UnsupportedPlatfromForAppInfo(string appFilePath, PlatformName platform)
				: base(platform + ": " + appFilePath) { }
		}

		public static AppInfo ToAppInfo(this AppBuildResult buildResult, string storageDirectory)
		{
			return CreateAppInfo(Path.Combine(storageDirectory, buildResult.PackageFileName),
				buildResult.Platform, buildResult.PackageGuid);
		}

		public static string GetFullAppNameForEngineApp(this AppInfo appInfo)
		{
			return appInfo is AndroidAppInfo ? "net.DeltaEngine." + appInfo.Name : appInfo.Name;
		}
	}
}
