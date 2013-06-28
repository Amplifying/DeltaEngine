using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the Visual Studio Template.
	/// </summary>
	public class VsTemplateTests
	{
		[Test]
		public void CreateWithEmptyGameTemplate()
		{
			var template = VsTemplate.GetEmptyGame(CreateFileSystemMock());
			Assert.AreEqual(templateZipMockPath, template.PathToZip);
			Assert.AreEqual(assemblyInfo, template.AssemblyInfo);
			Assert.AreEqual(csproj, template.Csproj);
			Assert.AreEqual(ico, template.Ico);
			Assert.AreEqual(2, template.SourceCodeFiles.Count);
			Assert.AreEqual(program, template.SourceCodeFiles[0]);
			Assert.AreEqual(game, template.SourceCodeFiles[1]);
		}

		private MockFileSystem CreateFileSystemMock()
		{
			var files = new Dictionary<string, MockFileData>();
			files.Add(templateZipMockPath,
				new MockFileData(File.ReadAllText(Path.Combine("NewDeltaEngineProject", "EmptyGame.zip"))));
			var fileSystem = new MockFileSystem(files);
			fileSystem.Directory.SetCurrentDirectory(templateZipMockPath);
			return fileSystem;
		}

		private const string BasePath =
			@"D:\Development\DeltaEngine\VisualStudioTemplates\Delta Engine";
		private readonly string templateZipMockPath = Path.Combine(BasePath, "EmptyGame.zip");
		private readonly string assemblyInfo = Path.Combine(BasePath, "Properties", "AssemblyInfo.cs");
		private readonly string csproj = Path.Combine(BasePath, "EmptyGame.csproj");
		private readonly string ico = Path.Combine(BasePath, "EmptyGameIcon.ico");
		private readonly string program = Path.Combine(BasePath, "Program.cs");
		private readonly string game = Path.Combine(BasePath, "Game.cs");

		[Test]
		public void CheckTotalNumberOfFilesFromEmptyGameTemplate()
		{
			var template = VsTemplate.GetEmptyGame(CreateFileSystemMock());
			var list = template.GetAllFilePathsAsList();
			Assert.AreEqual(5, list.Count);
		}
	}
}