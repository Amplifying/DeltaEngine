using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class PathExtensionsTests
	{
		[Test]
		public void GetDeltaEngineDirectory()
		{
			Assert.IsTrue(Directory.Exists(PathExtensions.GetDeltaEngineDirectory()));
		}

		[Test]
		public void GetDeltaEngineSolutionFilePath()
		{
			Assert.IsTrue(File.Exists(PathExtensions.GetEngineSolutionFilePath()));
		}

		[Test]
		public void GetSamplesSolutionFilePath()
		{
			Assert.IsTrue(File.Exists(PathExtensions.GetSamplesSolutionFilePath()));
		}
	}
}
