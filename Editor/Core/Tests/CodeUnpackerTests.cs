using System;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class CodeUnpackerTests
	{
		[Test]
		public void NoPackedDataSpecifiedException()
		{
			Assert.Throws<CodeUnpacker.NoPackedDataSpecified>(() => new CodeUnpacker(null));
			Assert.Throws<CodeUnpacker.NoPackedDataSpecified>(() => new CodeUnpacker(new byte[0]));
		}

		[Test]
		public void ExpectExceptionWhenUnpackEmptyFolder()
		{
			const int FilesToUnpack = 0;
			Assert.Throws<CodeUnpacker.NoSourceCodeFilesSentUnableToBuildApp>(
				() => new CodeUnpacker(BitConverter.GetBytes(FilesToUnpack)).SaveToDirectory("AnyFolder"));
		}

		[Test]
		public void UnpackPackedData()
		{
			CodePacker packer = GetCodePackerWithTestData();
			const string TestFolder = "UnpackedCode";
			new CodeUnpacker(packer.GetPackedData()).SaveToDirectory(TestFolder);
			Assert.AreEqual(packer.CollectedFilesToPack.Count, GetAllFiles(TestFolder).Length);
		}

		private static CodePacker GetCodePackerWithTestData()
		{
			return new CodePacker(PathExtensions.GetEngineSolutionFilePath(), "DeltaEngine.Platforms");
		}

		private static string[] GetAllFiles(string directory)
		{
			return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
		}
	}
}