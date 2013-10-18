using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using DeltaEngine.Core;
using DeltaEngine.Editor.AppBuilder.WindowsPhone7;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Logging;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public static class AppBuilderTestExtensions
	{
		public static T CloneViaBinaryData<T>(this T dataObjectToClone)
		{
			using (var dataStream = new MemoryStream())
			{
				var writer = new BinaryWriter(dataStream);
				BinaryDataExtensions.Save(dataObjectToClone, writer);
				dataStream.Seek(0, SeekOrigin.Begin);
				var reader = new BinaryReader(dataStream);
				var clonedObject = (T)reader.Create();
				return clonedObject;
			}
		}

		public static AppBuildMessage AsBuildTestWarning(string warningMessage)
		{
			var randomizer = new Random();
			var message = new AppBuildMessage(warningMessage)
			{
				Type = AppBuildMessageType.BuildWarning,
				Project = TestProjectName,
				Filename = "TestClass.cs",
				TextLine = randomizer.Next(1, 35).ToString(CultureInfo.InvariantCulture),
				TextColumn = randomizer.Next(1, 80).ToString(CultureInfo.InvariantCulture),
			};

			return message;
		}

		public const string TestProjectName = "TestProject";

		public static AppBuildMessage AsBuildTestError(string errorMessage)
		{
			var message = AsBuildTestWarning(errorMessage);
			message.Type = AppBuildMessageType.BuildError;
			return message;
		}

		public static AppInfo GetMockAppInfo(string appName, PlatformName platform,
			string directory = "")
		{
			return AppInfoExtensions.CreateAppInfo(Path.Combine(directory, appName + ".mockApp"),
				platform, Guid.NewGuid(), DateTime.Now);
		}

		public static AppInfo TryGetAlreadyBuiltApp(string appName, PlatformName platform)
		{
			var appsStorage = new BuiltAppsListViewModel();
			foreach (AppInfo app in appsStorage.BuiltApps)
				if (app.Name == appName && app.Platform == platform)
					return app;
			AppInfo downloadedApp = TryGetBuiltAppFromWebserver(appName, platform);
			appsStorage.AddApp(downloadedApp);
			return downloadedApp;
		}

		private static AppInfo TryGetBuiltAppFromWebserver(string appName, PlatformName platform)
		{
			var downloadClient = new WebClient();
			string appFileName = appName + GetAppFileExtension(platform);
			string sourceUrl = Path.Combine("http://DeltaEngine.net/", "BuiltSampleApps", appFileName);
			try
			{
				downloadClient.DownloadFile(sourceUrl, appFileName);
				return AppInfoExtensions.CreateAppInfo(appFileName, platform, GetAppGuidOfTestApp(appName),
					DateTime.Now);
			}
			catch (Exception ex)
			{
				Logger.Warning(ex);
				throw new AppNotFoundForPlatform(appName, platform);
			}
		}

		private static string GetAppFileExtension(PlatformName platform)
		{
			switch (platform)
			{
				case PlatformName.Android:
					return ".apk";
				case PlatformName.WindowsPhone7:
					return ".xap";
				case PlatformName.Web:
					return ".html";
				default:
					throw new UnsupportedPlatfrom(platform);
			}
		}

		private class UnsupportedPlatfrom : Exception
		{
			public UnsupportedPlatfrom(PlatformName platform)
				: base(platform.ToString()) { }
		}

		private static Guid GetAppGuidOfTestApp(string appName)
		{
			appName = appName.ToLower();
			if (appName.StartsWith("logoapp"))
				return new Guid("4d33a50e-3aa2-4e7e-bc0c-4ef7b3d5e985");
			throw new GuidForAppNotFound(appName);
		}

		private class GuidForAppNotFound : Exception
		{
			public GuidForAppNotFound(string appName)
				: base(appName) {}
		}

		public class AppNotFoundForPlatform : Exception
		{
			public AppNotFoundForPlatform(string appName, PlatformName platform)
				: base(appName + " for " + platform) {}
		}

		public static void LaunchLogoAppOnPrimaryDevice(PlatformName platform)
		{
			new ConsoleLogger();
			AppInfo app = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", platform);
			app.LaunchAppOnPrimaryDevice();
		}

		public static void LaunchLogoAppOnEmulatorDevice(PlatformName platform)
		{
			new ConsoleLogger();
			AppInfo app = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", platform);
			Device emulator = app.AvailableDevices.FirstOrDefault(device => device.IsEmulator);
			app.LaunchAppOnDevice(emulator);
		}
	}
}