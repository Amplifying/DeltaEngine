using System;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class PathExtensionsTests
	{
		[Test]
		public void InstallerSetsDeltaEnginePathEnvironmentVariable()
		{
			MakeSureEnvironmentVariableIsSet();
			Assert.IsTrue(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
		}

		private static void MakeSureEnvironmentVariableIsSet()
		{
			if (!PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
				SetEnvironmentVariable(PathExtensions.GetFallbackEngineSourceCodeDirectory());
		}

		private static void SetEnvironmentVariable(string value)
		{
			Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName, value);
		}

		[Test]
		public void WithoutInstallerDeltaEnginePathEnvironmentVariableIsNotSet()
		{
			DeleteEnvironmentVariableIfSet();
			Assert.IsFalse(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
		}

		private static void DeleteEnvironmentVariableIfSet()
		{
			if (PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
				SetEnvironmentVariable("");
		}

		[Test]
		public void DeltaEnginePathEnvironmentVariableMustBeAnExistingDirectory()
		{
			MakeSureEnvironmentVariableIsSet();
			Assert.IsTrue(Directory.Exists(PathExtensions.GetDeltaEngineInstalledDirectory()));
		}

		[Test]
		public void DefaultDeltaEngineSourceCodeDirectoryShouldBeEitherCodeOrDevelopment()
		{
			string defaultSourceCodeDirectory = PathExtensions.GetFallbackEngineSourceCodeDirectory();
			Assert.IsTrue(defaultSourceCodeDirectory == @"C:\Code\DeltaEngine" ||
				defaultSourceCodeDirectory == @"C:\Development\DeltaEngine");
		}

		[Test]
		public void InvalidStartupPathWillBeFixed()
		{
			Directory.SetCurrentDirectory(@"C:\");
			var fixedPath = PathExtensions.GetFallbackEngineSourceCodeDirectory();
			Assert.IsTrue(Directory.Exists(fixedPath), fixedPath);
		}

		[Test]
		public void DeltaEngineSolutionFileHasToBeAvailable()
		{
			string deltaEngineSolutionFilePath = PathExtensions.GetEngineSolutionFilePath();
			Assert.IsTrue(deltaEngineSolutionFilePath.Contains(DeltaEngineSolutionFilename));
			Assert.IsTrue(File.Exists(deltaEngineSolutionFilePath));
		}

		private const string DeltaEngineSolutionFilename = "DeltaEngine.sln";

		[Test]
		public void SamplesSolutionFileHasToBeAvailable()
		{
			string samplesSolutionFilePath = PathExtensions.GetSamplesSolutionFilePath();
			Assert.IsTrue(samplesSolutionFilePath.Contains(SamplesSolutionFilename));
			Assert.IsTrue(File.Exists(samplesSolutionFilePath));
		}

		private const string SamplesSolutionFilename = "DeltaEngine.Samples.sln";

		[Test]
		public void CheckSamplesSolutionFilePathFromSourceCodeDirectory()
		{
			string expectedFilePath = Path.Combine(
				PathExtensions.GetFallbackEngineSourceCodeDirectory(), DeltaEngineSolutionFilename);
			Assert.AreEqual(expectedFilePath.ToLower(),
				PathExtensions.GetFilePathFromSourceCode(DeltaEngineSolutionFilename).ToLower());
		}

		[Test, EmulateInstaller]
		public void CheckSamplesSolutionFilePathFromInstallerDirectory()
		{
			var expectedFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DeltaEngine", "OpenTK",
				SamplesSolutionFilename);
			Assert.AreEqual(expectedFilePath,
				PathExtensions.GetFilePathFromInstallerRelease(SamplesSolutionFilename));
		}

		private class EmulateInstaller : TestActionAttribute
		{
			public EmulateInstaller()
			{
				installerPath = Path.Combine(Directory.GetCurrentDirectory(), "DeltaEngine");
				frameworkPath = Path.Combine(installerPath, "OpenTK");
				solutionFilePath = Path.Combine(frameworkPath, SamplesSolutionFilename);
			}

			private readonly string installerPath;
			private readonly string frameworkPath;
			private readonly string solutionFilePath;

			public override void BeforeTest(TestDetails details)
			{
				if (Directory.Exists(installerPath))
					throw new DirectoryAlreadyExists(installerPath); //ncrunch: no coverage
				SetEnvironmentVariable(installerPath);
				Directory.CreateDirectory(installerPath);
				Directory.CreateDirectory(frameworkPath);
				using (File.Create(solutionFilePath)) {}
			}

			//ncrunch: no coverage start
			private class DirectoryAlreadyExists : Exception
			{
				public DirectoryAlreadyExists(string directory)
					: base(directory) {}
			} //ncrunch: no coverage end

			public override void AfterTest(TestDetails details)
			{
				SetEnvironmentVariable("");
				if (File.Exists(solutionFilePath))
					File.Delete(solutionFilePath);
				Directory.Delete(installerPath, true);
			}
		}

		[Test]
		public void WithoutEnvironmentVariableDefaultSourceCodePathShouldBeReturned()
		{
			DeleteEnvironmentVariableIfSet();
			var defaultSourcePath = PathExtensions.GetInstalledOrFallbackEnginePath();
			Assert.IsTrue(defaultSourcePath == @"C:\Code\DeltaEngine" ||
				defaultSourcePath == @"C:\Development\DeltaEngine");
		}

		[Test]
		public void VisualStudioProjectsFolderInMyDocumentsMustBeAvailable()
		{
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string visualStudioProjectsPath = Path.Combine(myDocumentsPath, "Visual Studio 2012",
				"Projects");
			Assert.AreEqual(visualStudioProjectsPath, PathExtensions.GetVisualStudioProjectsFolder());
			Assert.True(Directory.Exists(PathExtensions.GetVisualStudioProjectsFolder()));
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void EngineSolutionCannotBeFoundWithWorkingDirectorySetOutsideOfTheSourceCode()
		{
			Directory.SetCurrentDirectory(Directory.Exists(@"C:\Code") ? @"C:\Code" : @"C:\Development");
			Assert.Throws<FileNotFoundException>(() => PathExtensions.GetEngineSolutionFilePath());
		}

		[Test, Ignore]
		public void PrintDeltaEnginePathEnvironmentVariableToConsole()
		{
			string deltaEnginePath = PathExtensions.GetDeltaEngineInstalledDirectory();
			if (deltaEnginePath == null)
				throw new EnvironmentVariableNotFound(PathExtensions.EnginePathEnvironmentVariableName);
			Console.WriteLine("DeltaEnginePath: " + deltaEnginePath);
			Assert.IsTrue(deltaEnginePath.Contains("DeltaEngine"));
		}

		private class EnvironmentVariableNotFound : Exception
		{
			public EnvironmentVariableNotFound(string environmentVariableName)
				: base(environmentVariableName) {}
		}
	}
}