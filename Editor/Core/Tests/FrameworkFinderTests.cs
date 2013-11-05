using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	[Category("Slow")]
	public class FrameworkFinderTests
	{
		//ncrunch: no coverage start
		[SetUp]
		public void SetEnvironmentVariable()
		{
			Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName, "");
		}

		[Test]
		public void TestOutputDirectoryIsNotAFrameworkDirectory()
		{
			Assert.IsFalse(
				FrameworkFinder.IsFrameworkDirectory(new DirectoryInfo(Directory.GetCurrentDirectory())));
		}

		[Test]
		public void CheckAvailableDeltaEngineFrameworks()
		{
			CreateFrameworkFolders(GetAllDeltaEngineFrameworks());
			frameworks = new FrameworkFinder();
			Assert.AreEqual(6, frameworks.All.Length);
		}

		private static IEnumerable<string> GetAllDeltaEngineFrameworks()
		{
			return new[] { "GLFW", "MonoGame", "OpenTK", "SharpDX", "SlimDX", "Xna" };
		}

		private void CreateFrameworkFolders(IEnumerable<string> frameworksToCreate)
		{
			installerDirectories.AddRange(frameworksToCreate);
			foreach (var directoryName in installerDirectories)
			{
				Directory.CreateDirectory(directoryName);
				foreach (var additionalSubFolder in additionalSubFolders)
					Directory.CreateDirectory(Path.Combine(directoryName, additionalSubFolder));
			}
		}

		private readonly List<string> installerDirectories = new List<string>();
		private readonly string[] additionalSubFolders = new[] { "Samples", "VisualStudioTemplates" };
		private FrameworkFinder frameworks;

		[TearDown]
		public void DeleteFrameworkFolders()
		{
			foreach (var directoryName in installerDirectories)
			{
				foreach (var additionalSubFolder in additionalSubFolders)
				{
					var folderToDelete = Path.Combine(directoryName, additionalSubFolder);
					if (Directory.Exists(folderToDelete))
						Directory.Delete(folderToDelete);
				}
				if (Directory.Exists(directoryName))
					Directory.Delete(directoryName);
			}
		}

		[Test]
		public void CheckAvailabilityOfDefaultFramework()
		{
			CreateFrameworkFolders(GetAllDeltaEngineFrameworks());
			frameworks = new FrameworkFinder();
			Assert.AreEqual(DefaultFramework, frameworks.Default);
		}

		private const DeltaEngineFramework DefaultFramework = DeltaEngineFramework.OpenTK;

		[Test]
		public void InvalidFrameworkStructureDoesNotShowUp()
		{
			Directory.CreateDirectory(DefaultFramework.ToString());
			var invalidFrameworks = new FrameworkFinder();
			Assert.AreEqual(0, invalidFrameworks.All.Length);
			Directory.Delete(DefaultFramework.ToString());
		}

		[Test, Ignore]
		public void ThrowIfDefaultFrameworkIsNotAvailable()
		{
			CreateFrameworkFolders(GetDeltaEngineFrameworksWithoutDefault());
			frameworks = new FrameworkFinder();
			var defaultFramework = DeltaEngineFramework.Default;
			Assert.Throws<FrameworkFinder.EditorDefaultFrameworkNotInstalled>(
				() => { defaultFramework = frameworks.Default; });
			Assert.AreEqual(DeltaEngineFramework.Default, defaultFramework);
		}

		private static IEnumerable<string> GetDeltaEngineFrameworksWithoutDefault()
		{
			return new[] { "GLFW", "MonoGame", "SharpDX", "SlimDX", "Xna" };
		}
	}
}