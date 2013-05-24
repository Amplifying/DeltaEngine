using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	internal class CodeDataTests
	{
		[Test]
		public void CheckPrepareDummyFiles()
		{
			const string TestFolder = "LoadCodeFilesTest";
			if (Directory.Exists(TestFolder))
				Directory.Delete(TestFolder, true);
			PrepareDummyCodeFiles(TestFolder);

			Assert.IsTrue(Directory.Exists(TestFolder));
			string[] foundFiles = GetAllFiles(TestFolder);
			Assert.AreEqual(4, foundFiles.Length);
		}

		private static string[] GetAllFiles(string directory)
		{
			return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
		}

		private static void PrepareDummyCodeFiles(string testDirectory)
		{
			string subDir = Path.Combine(testDirectory, "Sub");
			string binDir = Path.Combine(subDir, "bin");
			if (!Directory.Exists(binDir))
				Directory.CreateDirectory(binDir);
			string packageDir = Path.Combine(testDirectory, "packages");
			if (!Directory.Exists(packageDir))
				Directory.CreateDirectory(packageDir);

			File.WriteAllText(Path.Combine(testDirectory, "Dummy1.cs"), GetDummyClassCode("Dummy1"));
			File.WriteAllText(Path.Combine(subDir, "Dummy2.cs"), GetDummyClassCode("Dummy2"));
			File.WriteAllText(Path.Combine(binDir, "Dummy3.cs"), GetDummyClassCode("Dummy3"));
			File.WriteAllText(Path.Combine(packageDir, "Dummy4.cs"), GetDummyClassCode("Dummy4"));
		}

		private static string GetDummyClassCode(string className)
		{
			return "namespace A { class " + className + " {} }";
		}

		[Test]
		public void LoadCodeFiles()
		{
			const string TestSourceFolder = "LoadCodeFilesTestSource";
			PrepareDummyCodeFiles(TestSourceFolder);

			byte[] directoryAsBytes = new CodeData(TestSourceFolder).GetBytes();
			Assert.AreNotEqual(0, directoryAsBytes.Length);

			const string TestTargetFolder = "LoadCodeFilesTestTarget";
			new CodeData(directoryAsBytes).SaveToDirectory(TestTargetFolder);
			Assert.AreEqual(2, GetAllFiles(TestTargetFolder).Length);
		}
	}
}