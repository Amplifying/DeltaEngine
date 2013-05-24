using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuildResultTests
	{
		[Test]
		public void CompareBuildResults()
		{
			BuildResult result = GetTestBuildResult();
			BuildResult sameResult = GetTestBuildResult();
			Assert.AreEqual(result, sameResult);
			Assert.AreNotEqual(result, new object());
			Assert.AreEqual(result.GetHashCode(), sameResult.GetHashCode());
		}

		private static BuildResult GetTestBuildResult()
		{
			return new BuildResult(PlatformName.WindowsPhone7.GetTestPackageForWP7());
		}

		[Test]
		public void SaveAndLoadGoodResult()
		{
			BuildResult goodResult = GetTestBuildResult();
			var loadedRequest = goodResult.CloneViaBinaryData();
			Assert.AreEqual(goodResult, loadedRequest);
		}

		[Test]
		public void SaveAndLoadBadResult()
		{
			var badResult = new BuildResult
			{
				Platform = PlatformName.WindowsPhone7,
				BuildError = "An has error happened."
			};
			var loadedRequest = badResult.CloneViaBinaryData();
			Assert.AreEqual(badResult, loadedRequest);
		}
	}
}