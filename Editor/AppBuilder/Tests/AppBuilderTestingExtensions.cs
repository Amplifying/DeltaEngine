using System;
using System.Globalization;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public static class AppBuilderTestingExtensions
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
			appsStorage.Load();
			foreach (AppInfo app in appsStorage.BuiltApps)
				if (app.Name == appName && app.Platform == platform)
					return app;
			return null;
		}

		/*wtf?
		public static AppInfo TryGetPrebuiltAppFromDatabase(this string appName,
			PlatformName platform)
		{
			string fileName = appName + PlatformNameExtensions.GetAppExtension(platform);
			string filePath = Path.Combine(@"\\Win7-VM\BuildServiceServer\AppDatabase", fileName);
			return File.Exists(filePath)
				? AppInfoExtensions.CreateAppInfo(filePath, platform, GetAppGuid(appName), DateTime.Now)
				: null;
		}

		private static Guid GetAppGuid(string appName)
		{
			switch (appName.ToLower())
			{
			case "logoapp":
				return new Guid("4d33a50e-3aa2-4e7e-bc0c-4ef7b3d5e985");
			case "breakout":
				return new Guid("2e78abad-fe79-455a-8393-48e08de57064");
			case "blocks":
				return new Guid("8d02900e-a9a6-4510-acd1-f8df74602ed0");
			default:
				return Guid.Empty;
			}
		}
		 */
	}
}