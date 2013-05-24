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
			var template = VsTemplate.GetEmptyGame();
			return new ProjectCreator(project, template,
				CreateEmptyGameFileSystemMock(project, template));
		}

		private static IFileSystem CreateEmptyGameFileSystemMock(CsProject project,
			VsTemplate template)
		{
			List<string> files = GetMockFileDataFromZip(template.PathToZip);
			Assert.AreEqual(5, files.Count);
			return
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{ template.AssemblyInfo, files[0] },
					{ template.Csproj, files[1] },
					{ template.Ico, files[2] },
					{ template.SourceCodeFiles[0], files[3] },
					{ template.SourceCodeFiles[1], files[4] },
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
			var template = VsTemplate.GetEmptyGame();
			return new ProjectCreator(new CsProject(), template, CreateCorruptFileSystemMock(template));
		}

		private static IFileSystem CreateCorruptFileSystemMock(VsTemplate template)
		{
			return
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{ "C:\\Foo\\AssemblyInfo.cs", new MockFileData("Assembly information") },
					{ template.SourceCodeFiles[0], new MockFileData("using System;") },
					{ "C:\\Bar\\DeltaEngine\\", new MockFileData(";") },
					{ template.SourceCodeFiles[1], new MockFileData("") }
				});
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
			const string ProjectToCompare =
				@"C:\Code\DeltaEngine\Editor\ProjectCreator\Tests\GeneratedProjectToCompare\";
			string locationPath = project.Location + project.Name + @"\";
			string pathToAssemblyInfo = locationPath + "Properties\\AssemblyInfo.cs";
			string pathToCsproj = locationPath + "NewDeltaEngineProject.csproj";
			string pathToIcon = locationPath + "NewDeltaEngineProjectIcon.ico";
			return
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{ pathToAssemblyInfo, GetMockFileData(ProjectToCompare + "Properties\\AssemblyInfo.cs") },
					{ pathToCsproj, GetMockFileData(ProjectToCompare + "NewDeltaEngineProject.csproj") },
					{ pathToIcon, GetMockFileData(ProjectToCompare + "NewDeltaEngineProjectIcon.ico") },
					{ locationPath + "Program.cs", GetMockFileData(ProjectToCompare + "Program.cs") },
					{ locationPath + "Game.cs", GetMockFileData(ProjectToCompare + "Game.cs") }
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
				"Properties\\AssemblyInfo.cs",
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