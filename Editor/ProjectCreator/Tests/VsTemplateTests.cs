using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the Visual Studio Template.
	/// </summary>
	public class VsTemplateTests
	{
		[TestFixtureSetUp]
		public void LoadResourcePathsOfEmptyApp()
		{
			string basePath = CreatorTestExtensions.GetEngineTemplatesDirectory();
			zipTemplateFilePath = Path.Combine(basePath, "EmptyApp.zip");
			assemblyInfoFilePath = Path.Combine(basePath, "Properties", "AssemblyInfo.cs");
			csprojFilePath = Path.Combine(basePath, "EmptyApp.csproj");
			icoFilePath = Path.Combine(basePath, "EmptyApp.ico");
			programClassFilePath = Path.Combine(basePath, "Program.cs");
			gameClassFilePath = Path.Combine(basePath, "ColorChanger.cs");
		}
		
		private string zipTemplateFilePath;
		private string assemblyInfoFilePath;
		private string csprojFilePath;
		private string icoFilePath;
		private string programClassFilePath;
		private string gameClassFilePath;

		[Test]
		public void CreateWithEmptyAppTemplate()
		{
			var template = VsTemplate.CreateByName(CreateFileSystemMock(), "EmptyApp");
			Assert.AreEqual(zipTemplateFilePath, template.PathToZip);
			Assert.AreEqual(assemblyInfoFilePath, template.AssemblyInfo);
			Assert.AreEqual(csprojFilePath, template.Csproj);
			Assert.AreEqual(icoFilePath, template.Ico);
			Assert.AreEqual(2, template.SourceCodeFiles.Count);
			Assert.IsTrue(template.SourceCodeFiles.Contains(programClassFilePath));
			Assert.IsTrue(template.SourceCodeFiles.Contains(gameClassFilePath));
		}

		private MockFileSystem CreateFileSystemMock()
		{
			var files = new Dictionary<string, MockFileData>();
			files.Add(zipTemplateFilePath,
				new MockFileData(File.ReadAllText(Path.Combine("GeneratedEmptyApp", "EmptyApp.zip"))));
			var fileSystem = new MockFileSystem(files);
			fileSystem.Directory.SetCurrentDirectory(zipTemplateFilePath);
			return fileSystem;
		}

		[Test]
		public void CheckTotalNumberOfFilesFromEmptyAppTemplate()
		{
			var template = VsTemplate.CreateByName(CreateFileSystemMock(), "EmptyApp");
			var list = template.GetAllFilePathsAsList();
			Assert.AreEqual(5, list.Count);
		}

		[Test]
		public void VsTemplatesHaveToBeAvailable()
		{
			Assert.Greater(VsTemplate.GetAllTemplateNames(DeltaEngineFramework.GLFW).Length, 0);
		}
	}
}