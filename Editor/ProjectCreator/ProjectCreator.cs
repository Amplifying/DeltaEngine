using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using SharpCompress.Archive;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Creates a new Delta Engine project on the drive based on a VisualStudioTemplate (.zip).
	/// </summary>
	public class ProjectCreator
	{
		public ProjectCreator(CsProject project, VsTemplate template, IFileSystem fileSystem)
		{
			Project = project;
			Template = template;
			FileSystem = fileSystem;
		}

		public CsProject Project { get; private set; }
		public VsTemplate Template { get; private set; }
		public IFileSystem FileSystem { get; private set; }

		public bool AreAllTemplateFilesAvailable()
		{
			return Template.GetAllFilePathsAsList().All(file => DoesFileExist(file));
		}

		private bool DoesFileExist(string path)
		{
			return FileSystem.File.Exists(path);
		}

		public void CreateProject()
		{
			if (!IsTargetDirectoryAvailable() || !IsSourceFileAvailable())
				return;

			CreateTargetDirectoryHierarchy();
			CopyTemplateFilesToLocation();
			ReplacePlaceholdersWithUserInput();
		}

		public bool IsTargetDirectoryAvailable()
		{
			return DoesDirectoryExist(Project.Location);
		}

		public bool IsSourceFileAvailable()
		{
			if (FileSystem.GetType() == typeof(FileSystem))
				return Template.PathToZip != string.Empty && ZipArchive.IsZipFile(Template.PathToZip);

			return Template.PathToZip != string.Empty &&
				FileSystem.Path.GetExtension(Template.PathToZip) == ".zip";
		}

		private bool DoesDirectoryExist(string path)
		{
			return FileSystem.Directory.Exists(path);
		}

		private void CreateTargetDirectoryHierarchy()
		{
			CreateDirectory(Project.Location + Project.Name + "\\");
			CreateDirectory(Project.Location + Project.Name + "\\Properties\\");
		}

		private void CreateDirectory(string path)
		{
			FileSystem.Directory.CreateDirectory(path);
		}

		private void CopyTemplateFilesToLocation()
		{
			if (FileSystem.GetType() == typeof(FileSystem))
			{
				var archive = ZipArchive.Open(Template.PathToZip);
				foreach (var entry in archive.Entries.Where(x => !x.FilePath.Contains("vstemplate")))
					CopyFileInZipToLocation(entry);
			}
			else
			{
				CopyFile(Template.AssemblyInfo,
					Project.Location + Project.Name + "\\Properties\\" + AssemblyInfo);
				CopyFile(Template.Csproj,
					Project.Location + Project.Name + "\\" + Project.Name + CsprojExtension);
				CopyFile(Template.Ico,
					Project.Location + Project.Name + "\\" + Project.Name + IcoSuffixAndExtension);
				foreach (var file in Template.SourceCodeFiles)
					CopyFile(file, Project.Location + Project.Name + "\\" + GetFileName(file));
			}
		}

		private void CopyFileInZipToLocation(IArchiveEntry entry)
		{
			using (var stream = entry.OpenEntryStream())
			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				FileSystem.File.WriteAllBytes(CreateTargetPathForZipEntry(entry), memoryStream.ToArray());
			}
		}

		private string CreateTargetPathForZipEntry(IEntry entry)
		{
			var target = Path.Combine(Project.Location, entry.FilePath).Replace('/', '\\');
			return target.Replace(FileSystem.Path.GetFileNameWithoutExtension(Template.PathToZip),
				Project.Name);
		}

		private void CopyFile(string sourceFileName, string destinationFileName)
		{
			FileSystem.File.Copy(sourceFileName, destinationFileName, true);
		}

		private void ReplacePlaceholdersWithUserInput()
		{
			ReplaceAssemblyInfo();
			ReplaceCsproj();
			ReplaceSourceCodeFile("Game.cs");
			ReplaceSourceCodeFile("Program.cs");
		}

		private void ReplaceAssemblyInfo()
		{
			var oldFile = ReadAllLines(Project.Location + Project.Name + "\\Properties\\" + AssemblyInfo);
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$projectname$", Project.Name));
			replacements.Add(new Replacement("$guid1$", Guid.NewGuid().ToString()));
			var newFile = ReplaceFile(oldFile, replacements);
			WriteAllText(Project.Location + Project.Name + "\\Properties\\" + AssemblyInfo, newFile);
		}

		private const string AssemblyInfo = "AssemblyInfo.cs";

		private IEnumerable<string> ReadAllLines(string path)
		{
			return FileSystem.File.ReadAllLines(path);
		}

		private static string ReplaceFile(IEnumerable<string> fileContent,
			List<Replacement> replacements)
		{
			var newFile = new StringBuilder();
			foreach (string line in fileContent)
				newFile.Append(ReplaceLine(line, replacements) + "\r\n");

			return newFile.ToString();
		}

		private static string ReplaceLine(string line, IEnumerable<Replacement> replacements)
		{
			return replacements.Aggregate(line,
				(current, replacement) => current.Replace(replacement.OldValue, replacement.NewValue));
		}

		private void WriteAllText(string path, string contents)
		{
			FileSystem.File.WriteAllText(path, contents);
		}

		private void ReplaceCsproj()
		{
			var oldFile =
				ReadAllLines(Project.Location + Project.Name + "\\" + Project.Name + CsprojExtension);
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$guid1$", ""));
			replacements.Add(new Replacement("$safeprojectname$", Project.Name));
			replacements.Add(new Replacement(GetFileName(Template.Ico),
				Project.Name + IcoSuffixAndExtension));
			replacements.Add(GetReplacementDependingOnFramework());
			var newFile = ReplaceFile(oldFile, replacements);
			WriteAllText(Project.Location + Project.Name + "\\" + Project.Name + CsprojExtension,
				newFile);
		}

		private const string CsprojExtension = ".csproj";

		private Replacement GetReplacementDependingOnFramework()
		{
			if (Project.Framework == DeltaEngineFramework.OpenTK)
				return new Replacement("WindowsOpenTK", "WindowsOpenTK");
			if (Project.Framework == DeltaEngineFramework.SharpDX)
				return new Replacement("WindowsOpenTK", "WindowsSharpDX");
			if (Project.Framework == DeltaEngineFramework.SlimDX)
				return new Replacement("WindowsOpenTK", "WindowsSlimDX");
			if (Project.Framework == DeltaEngineFramework.Xna)
				return new Replacement("WindowsOpenTK", "WindowsXna");

			return new Replacement("", "");
		}

		private void ReplaceSourceCodeFile(string sourceFileName)
		{
			var oldFile = ReadAllLines(Project.Location + Project.Name + "\\" + sourceFileName);
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$safeprojectname$", Project.Name));
			var newFile = ReplaceFile(oldFile, replacements);
			WriteAllText(Project.Location + Project.Name + "\\" + sourceFileName, newFile);
		}

		public bool HasDirectoryHierarchyBeenCreated()
		{
			return DoesDirectoryExist(Project.Location + Project.Name + "\\") &&
				DoesDirectoryExist(Project.Location + Project.Name + "\\Properties\\");
		}

		public bool HaveTemplateFilesBeenCopiedToLocation()
		{
			foreach (var file in Template.SourceCodeFiles)
				if (!DoesFileExist(Project.Location + Project.Name + "\\" + GetFileName(file)))
					return false;

			return DoesFileExist(Project.Location + Project.Name + "\\Properties\\" + AssemblyInfo) &&
				DoesFileExist(Project.Location + Project.Name + "\\" + Project.Name + CsprojExtension) &&
				DoesFileExist(Project.Location + Project.Name + "\\" + Project.Name + IcoSuffixAndExtension);
		}

		private string GetFileName(string path)
		{
			return FileSystem.Path.GetFileName(path);
		}

		private const string IcoSuffixAndExtension = "Icon.ico";
	}
}