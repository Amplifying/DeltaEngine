using System;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class ContinuousUpdaterViewModelTests
	{
		[SetUp]
		public void CreateViewModel()
		{
			updater = new ContinuousUpdaterViewModel(null);
		}

		private ContinuousUpdaterViewModel updater;

		[Test]
		public void GetAssemblyNameFromFolder()
		{
			Assert.AreEqual("DeltaEngine.Graphics.Tests",
				ContinuousUpdaterViewModel.GetAssemblyNameFromFolder(
					@"C:\code\DeltaEngine\Graphics\Tests\bin\Debug"));
		}

		[Test]
		public void FindAllEngineAssemblies()
		{
			Assert.GreaterOrEqual(updater.Assemblies.Count, 5);
			bool foundGraphicsTests = false;
			bool foundInputTests = false;
			bool foundMultimediaTests = false;
			bool foundPhysics2DTests = false;
			foreach (var assembly in updater.Assemblies)
			{
				if (assembly.Name == "Graphics.Tests")
					foundGraphicsTests = true;
				if (assembly.Name == "Input.Tests")
					foundInputTests = true;
				if (assembly.Name == "Multimedia.Tests")
					foundMultimediaTests = true;
				if (assembly.Name == "Physics2D.Tests")
					foundPhysics2DTests = true;
			}
			Assert.IsTrue(foundGraphicsTests);
			Assert.IsTrue(foundInputTests);
			Assert.IsTrue(foundMultimediaTests);
			Assert.IsTrue(foundPhysics2DTests);
		}

		[Test, Ignore]
		public void ShowAssemblyNames()
		{
			foreach (var assembly in updater.Assemblies)
				Console.WriteLine(assembly.FilePath);
		}

		[Test, Ignore]
		public void ShowTestNames()
		{
			//TODO: Does not work in test environment, probably not all types are loaded
			//updater.Select(updater.Assemblies[0]);
			//this works:
			updater.Select(new AssemblyFile(@"C:\code\DeltaEngine\Tests\bin\Debug\DeltaEngine.Tests.dll"));
			foreach (var test in updater.Tests)
				Console.WriteLine(test);
		}

		[Test, Ignore]
		public void GetVisualStudioPath()
		{
			var idePath = updater.GetLatestVisualStudioBinPath();
			Assert.IsTrue(idePath.EndsWith(@"Common7\IDE\"));
			Assert.AreEqual(
				idePath.Contains("11")
					? @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\"
					: @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\", idePath);
		}

		[Test, Ignore]
		public void GetLatestVisualStudioObject()
		{
			var dte = updater.GetLatestVisualStudioObject();
			Assert.IsNotNull(dte);
			Assert.AreEqual("Microsoft Visual Studio", dte.Name);
		}
	}
}