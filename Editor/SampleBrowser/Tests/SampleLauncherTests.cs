using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleLauncherTests
	{
		[SetUp]
		public void Init()
		{
			sampleLauncher = new SampleLauncher(GetTestAssembly());
			sample = GetTestSample();
		}

		private SampleLauncher sampleLauncher;

		private static Type GetTestAssembly()
		{
			Assembly assembly = Assembly.LoadFrom(GetPathToTestAssembly());
			return assembly.GetTypes().FirstOrDefault(type => type.Name == "TestClass");
		}

		private Sample sample;

		private static Sample GetTestSample()
		{
			return Sample.CreateTest("TestSample", @"Test\Test.csproj", GetPathToTestAssembly(),
				"TestClass", "TestMethod");
		}

		private static string GetPathToTestAssembly()
		{
			return Path.GetFullPath(Path.Combine("..", "..", "Assemblies", "TestAssembly.dll"));
		}

		[Test, Ignore]
		public void LaunchingInvalidSampleShouldThrow()
		{
			Assert.Throws<Win32Exception>(() => sampleLauncher.OpenProject(sample));
			Assert.Throws<FileNotFoundException>(() => sampleLauncher.StartExecutable(sample));
		}

		[Test, Ignore]
		public void FakeProjectShouldNotBeAvailable()
		{
			Assert.IsFalse(sampleLauncher.DoesProjectExist(sample));
		}

		[Test, Ignore]
		public void MockAssemblyShouldBeAvailable()
		{
			Assert.IsTrue(sampleLauncher.DoesAssemblyExist(sample));
		}
	}
}