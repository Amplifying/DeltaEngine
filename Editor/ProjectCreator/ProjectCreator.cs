using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using DeltaEngine.Editor.Core;
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
			if (!IsSourceFileAvailable())
				return;
			CreateTargetDirectoryHierarchy();
			CopyTemplateFilesToLocation();
			ReplacePlaceholdersWithUserInput();
		}

		public bool IsSourceFileAvailable()
		{
			if (FileSystem.GetType() == typeof(FileSystem))
				return Template.PathToZip != string.Empty && ZipArchive.IsZipFile(Template.PathToZip);
			return Template.PathToZip != string.Empty &&
				FileSystem.Path.GetExtension(Template.PathToZip) == ".zip";
		}

		private void CreateTargetDirectoryHierarchy()
		{
			FileSystem.Directory.CreateDirectory(Project.Path);
			FileSystem.Directory.CreateDirectory(Path.Combine(Project.Path, "Properties"));
		}

		private void CopyTemplateFilesToLocation()
		{
			if (FileSystem.GetType() == typeof(FileSystem))
			{
				var archive = ZipArchive.Open(Template.PathToZip);
				foreach (var entry in
					archive.Entries.Where(x => !x.FilePath.Contains("vstemplate") && !x.IsDirectory))
					CopyFileInZipToLocation(entry);
			}
			else
			{
				CopyFile(Template.AssemblyInfo,
					Path.Combine(Project.Path, "Properties", AssemblyInfo));
				CopyFile(Template.Csproj,
					Path.Combine(Project.Path, Project.Name + CsprojExtension));
				CopyFile(Template.Ico,
					Path.Combine(Project.Path, Project.Name + IcoSuffixAndExtension));
				foreach (var file in Template.SourceCodeFiles)
					CopyFile(file, Path.Combine(Project.Path, GetFileName(file)));
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
			var target = Path.Combine(Project.Path, entry.FilePath).Replace('/', '\\');
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
			foreach (var filename in Template.SourceCodeFiles)
				ReplaceSourceCodeFile(Path.GetFileName(filename));
		}

		private void ReplaceAssemblyInfo()
		{
			var oldFile =
				FileSystem.File.ReadAllLines(Path.Combine(Project.Path, "Properties", AssemblyInfo));
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$projectname$", Project.Name));
			replacements.Add(new Replacement("$guid1$", Guid.NewGuid().ToString()));
			var newFile = ReplaceFile(oldFile, replacements);
			FileSystem.File.WriteAllText(Path.Combine(Project.Path, "Properties", AssemblyInfo), newFile);
		}

		private const string AssemblyInfo = "AssemblyInfo.cs";

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

		private void ReplaceCsproj()
		{
			var oldFile =
				FileSystem.File.ReadAllLines(Path.Combine(Project.Path, Project.Name + CsprojExtension));
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$guid1$", ""));
			replacements.Add(new Replacement("$safeprojectname$", Project.Name));
			replacements.Add(new Replacement(GetFileName(Template.Ico),
				Project.Name + IcoSuffixAndExtension));
			replacements.Add(new Replacement(GetFileName(Template.Ico).Replace("Icon", ""),
				Project.Name + IcoSuffixAndExtension));
			replacements.AddRange(GetReplacementsDependingOnFramework());
			var newFile = ReplaceFile(oldFile, replacements);
			FileSystem.File.WriteAllText(Path.Combine(Project.Path, Project.Name + CsprojExtension),
				newFile);
		}

		private const string CsprojExtension = ".csproj";

		private IEnumerable<Replacement> GetReplacementsDependingOnFramework()
		{
			var replacements = new Replacement[2];
			replacements[0] = new Replacement(DeltaEngineFramework.OpenTK.ToInternalName(),
				Project.Framework.ToInternalName());
			replacements[1] = new Replacement(DeltaEngineFramework.OpenTK.ToString(),
				Project.Framework.ToString());
			return replacements;
		}

		private void ReplaceSourceCodeFile(string sourceFileName)
		{
			var oldFile = FileSystem.File.ReadAllLines(Path.Combine(Project.Path, sourceFileName));
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$safeprojectname$", Project.Name));
			var newFile = ReplaceFile(oldFile, replacements);
			FileSystem.File.WriteAllText(Path.Combine(Project.Path, sourceFileName), newFile);
		}

		public bool HasDirectoryHierarchyBeenCreated()
		{
			return FileSystem.Directory.Exists(Project.Path) &&
				FileSystem.Directory.Exists(Path.Combine(Project.Path, "Properties"));
		}

		public bool HaveTemplateFilesBeenCopiedToLocation()
		{
			foreach (var file in Template.SourceCodeFiles)
				if (!DoesFileExist(Path.Combine(Project.Path, GetFileName(file))))
					return false;
			return DoesFileExist(Path.Combine(Project.Path, "Properties", AssemblyInfo)) &&
				DoesFileExist(Path.Combine(Project.Path, Project.Name + CsprojExtension)) &&
				DoesFileExist(Path.Combine(Project.Path, Project.Name + IcoSuffixAndExtension));
		}

		private string GetFileName(string path)
		{
			return FileSystem.Path.GetFileName(path);
		}

		private const string IcoSuffixAndExtension = "Icon.ico";
	}
}