using System;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class PathExtensionsTests
	{
		[Test]
		public void DeltaEnginePathEnvironmentVariableMustBeSet()
		{
			Assert.IsTrue(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
		}

		[Test]
		public void DeltaEnginePathEnvironmentVariableMustBeAnExistingDirectory()
		{
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
			Assert.IsTrue(deltaEngineSolutionFilePath.Contains("DeltaEngine.sln"));
			Assert.IsTrue(File.Exists(deltaEngineSolutionFilePath));
		}

		[Test]
		public void SamplesSolutionFileHasToBeAvailable()
		{
			string samplesSolutionFilePath = PathExtensions.GetSamplesSolutionFilePath();
			Assert.IsTrue(samplesSolutionFilePath.Contains("DeltaEngine.Samples.sln"));
			Assert.IsTrue(File.Exists(samplesSolutionFilePath));
		}

		[Test]
		public void GetInstalledPathIfEnvironmentVariableIsSetAndGetSourceCodePathIfItIsNotSet()
		{
			MakeSureThatDeltaEngineEnvironmentVariableIsSet();
			Assert.AreEqual(PathExtensions.GetDeltaEngineInstalledDirectory(),
				PathExtensions.GetInstalledOrFallbackEnginePath());
			DeleteAndAssertDeltaEngineEnvironmentVariable();
			Assert.AreEqual(PathExtensions.GetFallbackEngineSourceCodeDirectory(),
				PathExtensions.GetInstalledOrFallbackEnginePath());
		}

		private static void MakeSureThatDeltaEngineEnvironmentVariableIsSet()
		{
			if (!PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
				SetAndAssertDeltaEngineEnvironmentVariable();
		}

		private static void SetAndAssertDeltaEngineEnvironmentVariable()
		{
			Assert.IsFalse(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
			Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName,
				PathExtensions.GetFallbackEngineSourceCodeDirectory());
			Assert.IsTrue(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
		}

		private static void DeleteAndAssertDeltaEngineEnvironmentVariable()
		{
			Assert.IsTrue(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
			Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName, "");
			Assert.IsFalse(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
		}

		[Test]
		public void EditorSourceCodeDirectoryHasToBeAvailable()
		{
			Assert.IsTrue(Directory.Exists(PathExtensions.GetEditorSourceCodeDirectory()));
		}

		//ncrunch: no coverage start
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