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
			foreach (AppInfo app in appsStorage.BuiltApps)
				if (app.Name == appName && app.Platform == platform)
					return app;
			return null;
		}
	}
}