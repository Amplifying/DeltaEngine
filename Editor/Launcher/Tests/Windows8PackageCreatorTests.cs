using System.IO;
using NCrunch.Framework;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute; 

namespace DeltaEngine.Editor.Launcher.Tests
{
	public class Windows8PackageCreatorTests
	{
		private readonly string projectBinDirectory = GetTestDirOrFile("Debug");
		private readonly string signingKey = GetTestDirOrFile("App1_TemporaryKey.pfx");
		private readonly string outputPath = GetTestDirOrFile("HelloWorld.appx");

		[SetUp]
		public void DeleteTestPackage()
		{
			if(File.Exists(outputPath))
				File.Delete(outputPath);
		}

		[Test]
		public void CreatePackageShouldThrowExpceptionOnInvalidPaths()
		{
			var packageCreator = new Windows8StorePackageCreator();
			Assert.Throws<DirectoryNotFoundException>(() => packageCreator.CreatePackage(@"X:\bin", outputPath));
			Assert.Throws<DirectoryNotFoundException>(
				() => packageCreator.CreatePackage(projectBinDirectory, @"X:\Test.appx"));
		}

		[Test]
		public void CreatePackageShouldThrowExpcetionWhenNoAppxManifestExist()
		{
			var packageCreator = new Windows8StorePackageCreator();
			Assert.Throws<Windows8StorePackageCreator.AppxManifestNotFound>(
				() => packageCreator.CreatePackage(GetTestDirOrFile("/"), outputPath));
		}

		[Test, Ignore]
		public void TestPackageCreation()
		{
			var packageCreator = new Windows8StorePackageCreator();
			packageCreator.CreatePackage(projectBinDirectory, outputPath);
			Assert.IsTrue(File.Exists(outputPath));
			Assert.IsFalse(packageCreator.IsPackageSigned(outputPath));
		}

		[Test, Ignore]
		public void SignPackageShouldTrowExceptionOnInvalidPaths()
		{
			var packageCreator = new Windows8StorePackageCreator();
			packageCreator.CreatePackage(projectBinDirectory, outputPath);
			Assert.Throws<FileNotFoundException>(
				() => packageCreator.SignPackage(@"X:\Test.appx", signingKey));
			Assert.Throws<FileNotFoundException>(
				() => packageCreator.SignPackage(outputPath, @"X:\Key.pfx"));
		}

		[Test, Ignore]
		public void TestSignPackage()
		{
			var packageCreator = new Windows8StorePackageCreator();
			packageCreator.CreatePackage(projectBinDirectory, outputPath);
			packageCreator.SignPackage(outputPath, signingKey);
			Assert.IsTrue(packageCreator.IsPackageSigned(outputPath));
		}

		private static string GetTestDirOrFile(string directoryOrFile)
		{
			string projectPath = Path.GetDirectoryName(NCrunchEnvironment.GetOriginalProjectPath());
			return Path.Combine(projectPath, "TestWindows8BinFiles",
				directoryOrFile);
		}
	}
}