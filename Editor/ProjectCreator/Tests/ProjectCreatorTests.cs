using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using NUnit.Framework;
using SharpCompress.Archive;
using SharpCompress.Archive.Zip;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the creation of Delta Engine C# projects.
	/// </summary>
	public class ProjectCreatorTests
	{
		[SetUp]
		public void Init()
		{
			valid = CreateWithValidFileSystemMock();
			invalid = CreateWithCorruptFileSystemMock();
		}

		private ProjectCreator valid;
		private ProjectCreator invalid;

		private static ProjectCreator CreateWithValidFileSystemMock()
		{
			var project = new CsProject();
			var template = VsTemplate.GetEmptyGame(CreateSolutionTemplateMock());
			return new ProjectCreator(project, template,
				CreateEmptyGameFileSystemMock(project, template));
		}

		private static MockFileSystem CreateSolutionTemplateMock()
		{
			const string TemplateZipMockPath = BasePath + @"\EmptyGame.zip";
			var files = new Dictionary<string, MockFileData>();
			files.Add(TemplateZipMockPath,
				new MockFileData(File.ReadAllText(Path.Combine("NewDeltaEngineProject", "EmptyGame.zip"))));
			var fileSystem = new MockFileSystem(files);
			fileSystem.Directory.SetCurrentDirectory(TemplateZipMockPath);
			return fileSystem;
		}

		private const string BasePath =
			@"D:\Development\DeltaEngine\VisualStudioTemplates\Delta Engine";

		private static IFileSystem CreateEmptyGameFileSystemMock(CsProject project,
			VsTemplate template)
		{
			List<string> files =
				GetMockFileDataFromZip(Path.Combine("NewDeltaEngineProject", "EmptyGame.zip"));
			Assert.AreEqual(5, files.Count);
			var fileSystem = new Dictionary<string, MockFileData>();
			fileSystem.Add(Path.Combine(BasePath, "EmptyGame.zip"),
				new MockFileData(File.ReadAllText(Path.Combine("NewDeltaEngineProject", "EmptyGame.zip"))));
			fileSystem.Add(Path.Combine(BasePath, template.AssemblyInfo), files[3]);
			fileSystem.Add(Path.Combine(BasePath, template.Csproj), files[4]);
			fileSystem.Add(Path.Combine(BasePath, template.Ico), files[0]);
			fileSystem.Add(Path.Combine(BasePath, template.SourceCodeFiles[0]), files[2]);
			fileSystem.Add(Path.Combine(BasePath, template.SourceCodeFiles[1]), files[1]);
			fileSystem.Add(project.Path, new MockDirectoryData());
			return new MockFileSystem(fileSystem);
		}

		private static List<string> GetMockFileDataFromZip(string pathToTemplate)
		{
			var files = new List<string>();
			Assert.IsTrue(ZipArchive.IsZipFile(pathToTemplate));
			var archive = ZipArchive.Open(pathToTemplate);
			foreach (var entry in archive.Entries.Where(x => !x.FilePath.Contains("vstemplate")))
				AddZippedFileAsStringToList(entry, files);
			return files;
		}

		private static void AddZippedFileAsStringToList(IArchiveEntry entry,
			ICollection<string> files)
		{
			using (var stream = entry.OpenEntryStream())
			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				using (var byteStream = new MemoryStream(memoryStream.ToArray()))
				using (var reader = new StreamReader(byteStream))
					files.Add(reader.ReadToEnd());
			}
		}

		private static ProjectCreator CreateWithCorruptFileSystemMock()
		{
			var template = VsTemplate.GetEmptyGame(CreateCorruptVisualStudioTemplateMock());
			return new ProjectCreator(new CsProject(), template, CreateCorruptFileSystemMock(template));
		}

		private static MockFileSystem CreateCorruptVisualStudioTemplateMock()
		{
			return
				new MockFileSystem(new Dictionary<string, MockFileData> { { "C", new MockFileData("") } });
		}

		private static IFileSystem CreateCorruptFileSystemMock(VsTemplate template)
		{
			var files = new Dictionary<string, MockFileData>();
			files.Add("C:\\Foo\\AssemblyInfo.cs", new MockFileData("Assembly information"));
			files.Add(template.SourceCodeFiles[0], new MockFileData("using System;"));
			files.Add("C:\\Bar\\DeltaEngine\\", new MockFileData(";"));
			files.Add(template.SourceCodeFiles[1], new MockFileData(""));
			return new MockFileSystem(files);
		}

		[Test]
		public void CheckAvailabilityOfTheTemplateFiles()
		{
			Assert.IsTrue(valid.AreAllTemplateFilesAvailable());
			Assert.IsFalse(invalid.AreAllTemplateFilesAvailable());
		}

		[Test]
		public void CheckAvailabilityOfTheSourceFile()
		{
			Assert.IsTrue(valid.IsSourceFileAvailable());
			Assert.IsFalse(invalid.IsSourceFileAvailable());
		}

		[Test]
		public void CheckIfFolderHierarchyIsCreatedCorrectly()
		{
			valid.CreateProject();
			Assert.IsTrue(valid.HasDirectoryHierarchyBeenCreated());
			invalid.CreateProject();
			Assert.IsFalse(invalid.HasDirectoryHierarchyBeenCreated());
		}

		[Test]
		public void CheckIfProjectFilesAreCopiedCorrectly()
		{
			valid.CreateProject();
			Assert.IsTrue(valid.HaveTemplateFilesBeenCopiedToLocation());
			invalid.CreateProject();
			Assert.IsFalse(invalid.HaveTemplateFilesBeenCopiedToLocation());
		}

		[Test]
		public void CheckIfAllPlaceholdersHaveBeenReplaced()
		{
			var mockFileSystem = CreateApprovedSystemMock(valid.Project);
			valid.CreateProject();
			Assert.IsTrue(CompareFileSystems(mockFileSystem, valid.FileSystem, valid.Project.Path));
		}

		private static IFileSystem CreateApprovedSystemMock(CsProject project)
		{
			const string GeneratedProjectToCompare = "NewDeltaEngineProject";
			string mockPathAssemblyInfo = Path.Combine(project.Path, "Properties", "AssemblyInfo.cs");
			string realPathAssemblyInfo = Path.Combine(GeneratedProjectToCompare, "Properties", 
				"AssemblyInfo.cs");
			string mockPathCsproj = Path.Combine(project.Path, "NewDeltaEngineProject.csproj");
			string realPathCsproj = Path.Combine(GeneratedProjectToCompare,
				"NewDeltaEngineProject.csproj");
			string mockPathIcon = Path.Combine(project.Path, "NewDeltaEngineProjectIcon.ico");
			string realPathIcon = Path.Combine(GeneratedProjectToCompare,
				"NewDeltaEngineProjectIcon.ico");
			string mockPathProgram = Path.Combine(project.Path, "Program.cs");
			string realPathProgram = Path.Combine(GeneratedProjectToCompare, "Program.cs");
			string mockPathGame = Path.Combine(project.Path, "Game.cs");
			string realPathGame = Path.Combine(GeneratedProjectToCompare, "Game.cs");
			return
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{ mockPathAssemblyInfo, GetMockFileData(realPathAssemblyInfo) },
					{ mockPathCsproj, GetMockFileData(realPathCsproj) },
					{ mockPathIcon, GetMockFileData(realPathIcon) },
					{ mockPathProgram, GetMockFileData(realPathProgram) },
					{ mockPathGame, GetMockFileData(realPathGame) }
				});
		}

		private static MockFileData GetMockFileData(string pathToFile)
		{
			return new MockFileData(new FileSystem().File.ReadAllText(pathToFile));
		}

		private static bool CompareFileSystems(IFileSystem fs1, IFileSystem fs2, string path)
		{
			var filesToCheck = new List<string>
			{
				Path.Combine("Properties", "AssemblyInfo.cs"),
				"NewDeltaEngineProject.csproj",
				"Program.cs",
				"Game.cs"
			};

			return filesToCheck.All(file => CompareFileInFileSystem(fs1, fs2, Path.Combine(path, file)));
		}

		private static bool CompareFileInFileSystem(IFileSystem fs1, IFileSystem fs2, string filePath)
		{
			return IsFileEqual(fs1.File.ReadAllLines(filePath), fs2.File.ReadAllLines(filePath), filePath);
		}

		private static bool IsFileEqual(IList<string> file1, IList<string> file2, string filePath)
		{
			if (file1.Count != file2.Count)
				throw new NumberOfTextLinesAreNotEqual(filePath);
			for (int i = 0; i < file1.Count; i++)
				if (file1[i] != file2[i] && !file2[i].Contains("Guid") &&
					!file2[i].Contains("AssemblyVersion") && !file2[i].Contains("AssemblyFileVersion"))
					throw new TextLineIsDifferent(filePath, file1[i], file2[i]);
			return true;
		}

		private class NumberOfTextLinesAreNotEqual : Exception
		{
			public NumberOfTextLinesAreNotEqual(string filePath)
				: base(Path.GetFileName(filePath)) { }
		}

		private class TextLineIsDifferent : Exception
		{
			public TextLineIsDifferent(string filePath, string expected, string actual)
				: base(Path.GetFileName(filePath) + " - expected: " + expected + ", actual: " + actual) { }
		}
	}
}