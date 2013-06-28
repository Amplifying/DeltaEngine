using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	[NUnit.Framework.Category("Slow")]
	public class SampleLauncherTests
	{
		[SetUp]
		public void Init()
		{
			sampleLauncher = new SampleLauncher(GetTestAssembly());
		}

		private SampleLauncher sampleLauncher;

		private static Type GetTestAssembly()
		{
			Assembly assembly = Assembly.LoadFrom(GetPathToTestAssembly(true));
			return assembly.GetTypes().FirstOrDefault(type => type.Name == "TestClass");
		}

		private static string GetPathToTestAssembly(bool isValid)
		{
			return isValid ? Path.GetFullPath("TestAssembly.dll") : @"Z:\invalid\TestAssembly.dll";
		}

		[Test]
		public void LaunchingInvalidSampleShouldThrow()
		{
			Assert.Throws<Win32Exception>(() => sampleLauncher.OpenProject(GetTestSample(false)));
			Assert.Throws<Win32Exception>(() => sampleLauncher.StartExecutable(GetGameSample(false)));
		}

		private static Sample GetTestSample(bool isValid)
		{
			return Sample.CreateTest("TestSample", @"Test\Test.csproj", GetPathToTestAssembly(isValid),
				"TestClass", "TestMethod");
		}

		private static Sample GetGameSample(bool isValid)
		{
			return Sample.CreateGame("TestSample", @"Test\Test.csproj", GetPathToTestAssembly(isValid));
		}

		[Test]
		public void FakeProjectShouldNotBeAvailable()
		{
			Assert.IsFalse(sampleLauncher.DoesProjectExist(GetTestSample(true)));
			Assert.IsFalse(sampleLauncher.DoesProjectExist(GetGameSample(true)));
		}

		[Test]
		public void MockAssemblyShouldBeAvailable()
		{
			Assert.IsTrue(sampleLauncher.DoesAssemblyExist(GetTestSample(true)));
			Assert.IsTrue(sampleLauncher.DoesAssemblyExist(GetGameSample(true)));
		}
	}
}