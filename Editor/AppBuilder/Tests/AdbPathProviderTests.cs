using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AdbPathProviderTests
	{
		[Test, Category("Slow")]
		public void FindAdbPath()
		{
			DeleteSupportFolderForTestIfExists();

			var provider = new AdbPathProvider();
			string adbPath = provider.GetAdbPath();
			Assert.IsTrue(File.Exists(adbPath));
		}

		private static void DeleteSupportFolderForTestIfExists()
		{
			if (Directory.Exists(AdbPathProvider.AndroidSupportFilesFolderName))
				Directory.Delete(AdbPathProvider.AndroidSupportFilesFolderName, true);
		}
	}
}