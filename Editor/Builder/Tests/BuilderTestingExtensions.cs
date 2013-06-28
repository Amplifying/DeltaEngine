using System;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Builder.Tests
{
	public static class BuilderTestingExtensions
	{
		public static BuildMessage AsBuildTestWarning(this string warningMessage)
		{
			var randomizer = new Random();
			var message = new BuildMessage(warningMessage)
			{
				Type = BuildMessageType.BuildWarning,
				Project = TestProjectName,
				Filename = "TestClass.cs",
				TextLine = randomizer.Next(1, 35),
				TextColumn = randomizer.Next(1, 80),
			};

			return message;
		}

		public const string TestProjectName = "TestProject";

		public static BuildMessage AsBuildTestError(this string errorMessage)
		{
			var message = AsBuildTestWarning(errorMessage);
			message.Type = BuildMessageType.BuildError;

			return message;
		}

		public static BuiltApp AsBuiltApp(this string appName, PlatformName platform)
		{
			return new BuiltApp { Name = appName, Platform = platform };
		}
	}
}