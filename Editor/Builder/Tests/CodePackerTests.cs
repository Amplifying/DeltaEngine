using System.IO;
using NCrunch.Framework;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class CodePackerTests
	{
		[Test]
		public void LoadCodeFromProject()
		{
			CodePacker packer = GetCodePackerWithBuilderTestsData();
			int expectedNumberOfPackedFiles = packer.CollectedFilesToPack.Count;
			Assert.AreNotEqual(0, expectedNumberOfPackedFiles);
		}

		private static CodePacker GetCodePackerWithBuilderTestsData()
		{
			string editorSolutionFilePath = NCrunchEnvironment.GetOriginalSolutionPath();
			string builderTestsProjectName = Path.GetFileNameWithoutExtension(
				NCrunchEnvironment.GetOriginalProjectPath());
			return new CodePacker(editorSolutionFilePath, builderTestsProjectName);
		}

		[Test]
		public void UnpackPackedData()
		{
			CodePacker packer = GetCodePackerWithBuilderTestsData();
			byte[] packedData = packer.GetPackedData();
			const string TestFolder = "UnpackedCode";
			try
			{
				new CodeUnpacker(packedData).SaveToDirectory(TestFolder);
				Assert.AreEqual(packer.CollectedFilesToPack.Count, GetAllFiles(TestFolder).Length);
			}
			finally
			{
				Directory.Delete(TestFolder, true);
			}
		}

		private static string[] GetAllFiles(string directory)
		{
			return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
		}
	}
}
