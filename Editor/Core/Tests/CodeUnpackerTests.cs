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

		// TODO:
		//[Test]
		public void ExpectExceptionWhenUnpackEmptyFolder()
		{
			const string Folder = "ExpectExceptionWhenPackEmptyFolder";
			try
			{
				Directory.CreateDirectory(Folder);
				CodePacker packer = new CodePacker(Folder);
				new CodeUnpacker(packer.GetPackedData()).SaveToDirectory("AnyDirectory");
			}
			finally 
			{
				if (Directory.Exists(Folder))
					Directory.Delete(Folder);
			}
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
