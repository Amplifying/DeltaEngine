using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace DeltaEngine.Editor.Common.Tests
{
	public class AssemblyCheckerTests
	{
		[SetUp]
		public void Init()
		{
			checker = new AssemblyChecker();
		}

		private AssemblyChecker checker;

		[Test]
		public void Create()
		{
			Assert.AreEqual(0, checker.OutdatedAssemblies.Count);
		}

		[Test]
		public void LoadOneOutdatedAssemblyToCurrentAppDomain()
		{
			LoadObsolescedAssembly();
			checker.CheckCurrentAppDomain();
			Assert.AreEqual(1, checker.OutdatedAssemblies.Count);
		}

		private void LoadObsolescedAssembly()
		{
			File.SetLastWriteTime(pathToTestAssembly, DateTime.Today.AddDays(-7));
			Assembly.LoadFile(pathToTestAssembly);
		}

		private readonly string pathToTestAssembly =
			Path.GetFullPath(Path.Combine("Content", "DeltaEngine.Editor.Common.dll"));
	}
}