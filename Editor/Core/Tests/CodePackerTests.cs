using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class CodePackerTests
	{
		[Test]
		public void NoSolutionPathSpecifiedException()
		{
			Assert.Throws<SolutionFileLoader.NoSolutionPathSpecified>(() => new CodePacker(null, "App"));
			Assert.Throws<SolutionFileLoader.NoSolutionPathSpecified>(() => new CodePacker("", "App"));
		}

		[Test]
		public void ProjectNotFoundInSolutionException()
		{
			Assert.Throws<SolutionFileLoader.ProjectNotFoundInSolution>(
				() => new CodePacker(PathExtensions.GetEngineSolutionFilePath(), "App"));
		}

		[Test]
		public void LoadCodeFromProject()
		{
			CodePacker packer = GetCodePackerWithBuilderTestsData();
			Assert.AreNotEqual(0, packer.CollectedFilesToPack.Count);
		}

		private static CodePacker GetCodePackerWithBuilderTestsData()
		{
			string solutionFilePath = PathExtensions.GetEngineSolutionFilePath();
			string testsProjectName = Path.GetFileNameWithoutExtension(solutionFilePath);
			return new CodePacker(solutionFilePath, testsProjectName);
		}
	}
}
