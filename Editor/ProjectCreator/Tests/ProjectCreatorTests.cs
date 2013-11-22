using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Mocks;
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
			var project = new CsProject("John Doe");
			var template = VsTemplate.CreateByName(CreateSolutionTemplateMock(), "EmptyApp");
			return new ProjectCreator(project, template, CreateMockService(),
				CreateEmptyAppFileSystemMock(project, template));
		}

		private static MockFileSystem CreateSolutionTemplateMock()
		{
			string emptyGameZipTemplateFilePath =
				Path.Combine(CreatorTestExtensions.GetEngineTemplatesDirectory(), "EmptyApp.zip");
			var files = new Dictionary<string, MockFileData>();
			files.Add(emptyGameZipTemplateFilePath,
				new MockFileData(File.ReadAllText(Path.Combine("GeneratedEmptyApp", "EmptyApp.zip"))));
			var fileSystem = new MockFileSystem(files);
			fileSystem.Directory.SetCurrentDirectory(emptyGameZipTemplateFilePath);
			return fileSystem;
		}

		private static IFileSystem CreateEmptyAppFileSystemMock(CsProject project,
			VsTemplate template)
		{
			List<string> files =
				GetMockFileDataFromZip(Path.Combine("GeneratedEmptyApp", "EmptyApp.zip"));
			Assert.AreEqual(5, files.Count);
			var fileSystem = new Dictionary<string, MockFileData>();
			string basePath = CreatorTestExtensions.GetEngineTemplatesDirectory();
			fileSystem.Add(Path.Combine(basePath, "EmptyApp.zip"),
				new MockFileData(File.ReadAllText(Path.Combine("GeneratedEmptyApp", "EmptyApp.zip"))));
			fileSystem.Add(Path.Combine(basePath, template.AssemblyInfo), files[3]);
			fileSystem.Add(Path.Combine(basePath, template.Csproj), files[4]);
			fileSystem.Add(Path.Combine(basePath, template.Ico), files[0]);
			fileSystem.Add(Path.Combine(basePath, template.SourceCodeFiles[0]), files[2]);
			fileSystem.Add(Path.Combine(basePath, template.SourceCodeFiles[1]), files[1]);
			fileSystem.Add(project.BaseDirectory, new MockDirectoryData());
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

		private static Service CreateMockService()
		{
			return new MockService("John Doe", "LogoApp");
		}

		private static ProjectCreator CreateWithCorruptFileSystemMock()
		{
			var template = VsTemplate.CreateByName(CreateCorruptVisualStudioTemplateMock(), "EmptyApp");
			return new ProjectCreator(new CsProject(""), template, CreateMockService(),
				CreateCorruptFileSystemMock(template));
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
			Assert.IsTrue(CompareFileSystems(mockFileSystem, valid.FileSystem, valid.Project.BaseDirectory));
		}

		private static IFileSystem CreateApprovedSystemMock(CsProject project)
		{
			const string GeneratedProjectToCompare = "GeneratedEmptyApp";
			string mockPathAssemblyInfo = Path.Combine(project.BaseDirectory, "Properties", "AssemblyInfo.cs");
			string realPathAssemblyInfo = Path.Combine(GeneratedProjectToCompare, "Properties",
				"AssemblyInfo.cs");
			string mockPathCsproj = Path.Combine(project.BaseDirectory, "GeneratedEmptyApp.csproj");
			string realPathCsproj = Path.Combine(GeneratedProjectToCompare,
				"GeneratedEmptyApp.csproj");
			string mockPathIcon = Path.Combine(project.BaseDirectory, "GeneratedEmptyAppIcon.ico");
			string realPathIcon = Path.Combine(GeneratedProjectToCompare,
				"GeneratedEmptyAppIcon.ico");
			string mockPathProgram = Path.Combine(project.BaseDirectory, "Program.cs");
			string realPathProgram = Path.Combine(GeneratedProjectToCompare, "Program.cs");
			string mockPathGame = Path.Combine(project.BaseDirectory, "Game.cs");
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
				"GeneratedEmptyApp.csproj",
				"Program.cs",
				"Game.cs"
			};
			return filesToCheck.All(file => CompareFileInFileSystem(fs1, fs2, Path.Combine(path, file)));
		}

		private static bool CompareFileInFileSystem(IFileSystem fs1, IFileSystem fs2, string filePath)
		{
			return IsFileEqual(fs1.File.ReadAllLines(filePath), fs2.File.ReadAllLines(filePath),
				filePath);
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
				: base(Path.GetFileName(filePath)) {}
		}

		private class TextLineIsDifferent : Exception
		{
			public TextLineIsDifferent(string filePath, string expected, string actual)
				: base(Path.GetFileName(filePath) + " - expected: " + expected + ", actual: " + actual) {}
		}
	}
}