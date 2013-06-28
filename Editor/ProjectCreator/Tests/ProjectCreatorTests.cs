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
			const string TemplateZipMockPath =
				@"D:\Development\DeltaEngine\VisualStudioTemplates\Delta Engine\EmptyGame.zip";
			var files = new Dictionary<string, MockFileData>();
			files.Add(TemplateZipMockPath,
				new MockFileData(File.ReadAllText(Path.Combine("NewDeltaEngineProject", "EmptyGame.zip"))));
			var fileSystem = new MockFileSystem(files);
			fileSystem.Directory.SetCurrentDirectory(TemplateZipMockPath);
			return fileSystem;
		}

		private static IFileSystem CreateEmptyGameFileSystemMock(CsProject project,
			VsTemplate template)
		{
			const string BasePath = @"D:\Development\DeltaEngine\VisualStudioTemplates\Delta Engine";
			List<string> files =
				GetMockFileDataFromZip(Path.Combine("NewDeltaEngineProject", "EmptyGame.zip"));
			Assert.AreEqual(5, files.Count);
			return
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						Path.Combine(BasePath, "EmptyGame.zip"),
						new MockFileData(File.ReadAllText(Path.Combine("NewDeltaEngineProject", "EmptyGame.zip")))
					},
					{ Path.Combine(BasePath, template.AssemblyInfo), files[3] },
					{ Path.Combine(BasePath, template.Csproj), files[4] },
					{ Path.Combine(BasePath, template.Ico), files[0] },
					{ Path.Combine(BasePath, template.SourceCodeFiles[0]), files[2] },
					{ Path.Combine(BasePath, template.SourceCodeFiles[1]), files[1] },
					{ project.Location, new MockDirectoryData() }
				});
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
		public void CheckAvailabilityOfTheTargetDirectory()
		{
			Assert.IsTrue(valid.IsTargetDirectoryAvailable());
			Assert.IsFalse(invalid.IsTargetDirectoryAvailable());
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
			string pathToLocation = valid.Project.Location + valid.Project.Name + "\\";
			Assert.IsTrue(CompareFileSystems(mockFileSystem, valid.FileSystem, pathToLocation));
		}

		private static IFileSystem CreateApprovedSystemMock(CsProject project)
		{
			const string GeneratedProjectToCompare = "NewDeltaEngineProject";
			string locationPath = Path.Combine(project.Location, project.Name);
			string mockPathAssemblyInfo = Path.Combine(locationPath, "Properties", "AssemblyInfo.cs");
			string realPathAssemblyInfo = Path.Combine(GeneratedProjectToCompare, "Properties",
				"AssemblyInfo.cs");
			string mockPathCsproj = Path.Combine(locationPath, "NewDeltaEngineProject.csproj");
			string realPathCsproj = Path.Combine(GeneratedProjectToCompare,
				"NewDeltaEngineProject.csproj");
			string mockPathIcon = Path.Combine(locationPath, "NewDeltaEngineProjectIcon.ico");
			string realPathIcon = Path.Combine(GeneratedProjectToCompare,
				"NewDeltaEngineProjectIcon.ico");
			string mockPathProgram = Path.Combine(locationPath, "Program.cs");
			string realPathProgram = Path.Combine(GeneratedProjectToCompare, "Program.cs");
			string mockPathGame = Path.Combine(locationPath, "Game.cs");
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

			return filesToCheck.All(file => CompareFileInFileSystem(fs1, fs2, path + file));
		}

		private static bool CompareFileInFileSystem(IFileSystem fs1, IFileSystem fs2, string file)
		{
			return FileEquals(fs1.File.ReadAllLines(file), fs2.File.ReadAllLines(file));
		}

		private static bool FileEquals(IList<string> file1, IList<string> file2)
		{
			return file1.Count == file2.Count &&
				!file1.Where((t, i) => t != file2[i] && !file2[i].Contains("Guid")).Any();
		}
	}
}