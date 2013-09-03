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

		[Test]
		public void ExpectExceptionWhenPackDirectoryWithoutCode()
		{
			const string Folder = "ExpectExceptionWhenPackEmptyFolder";
			try
			{
				Directory.CreateDirectory(Folder);
				Assert.Throws<CodePacker.NoCodeAvailableToPack>(() => new CodePacker(Folder));
			}
			finally
			{
				if (Directory.Exists(Folder))
					Directory.Delete(Folder);
			}
		}

		private static CodePacker GetCodePackerWithBuilderTestsData()
		{
			string solutionFilePath = PathExtensions.GetEngineSolutionFilePath();
			string testsProjectName = Path.GetFileNameWithoutExtension(solutionFilePath);
			return new CodePacker(solutionFilePath, testsProjectName);
		}
	}
}
