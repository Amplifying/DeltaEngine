using System;

namespace DeltaEngine.Editor.Builder.Tests
{
	public static class StringMessageExtensions
	{
		public static BuildMessage AsBuildTestInfoMessage(this string infoMessage)
		{
			return new BuildMessage(infoMessage)
			{
				Type = BuildMessageType.BuildInfo,
				Project = TestProjectName,
			};
		}

		public const string TestProjectName = "TestProject";

		public static BuildMessage AsBuildTestWarning(this string warningMessage)
		{
			var randomizer = new Random();
			var message = AsBuildTestInfoMessage(warningMessage);
			message.Type = BuildMessageType.BuildWarning;
			message.Filename = "TestClass.cs";
			message.TextLine = randomizer.Next(1, 35);
			message.TextColumn = randomizer.Next(1, 80);

			return message;
		}

		public static BuildMessage AsBuildTestError(this string errorMessage)
		{
			var message = AsBuildTestWarning(errorMessage);
			message.Type = BuildMessageType.BuildError;

			return message;
		}
	}
}