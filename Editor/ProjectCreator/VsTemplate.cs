using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using SharpCompress.Archive.Zip;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Template ZIP-File from which a new Delta Engine C# project will be created.
	/// </summary>
	public class VsTemplate
	{
		private VsTemplate(string templateName, IEnumerable<string> sourceCodeFileNames,
			IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			templatePath = Path.Combine("VisualStudioTemplates", "Delta Engine", templateName + ".zip");
			PathToZip = GetPathToVisualStudioTemplateZip(templateName);
			var basePath = GetBasePath(PathToZip);
			AssemblyInfo = Path.Combine(basePath, "Properties", "AssemblyInfo.cs");
			Csproj = Path.Combine(basePath, templateName + ".csproj");
			Ico = Path.Combine(basePath, templateName + "Icon.ico");
			SourceCodeFiles = new List<string>();
			var csFileNames = new List<string>();
			foreach (var csFileName in ZipArchive.Open(PathToZip).Entries)
			{
				var filename = csFileName.FilePath;
				if (!filename.Contains("AssemblyInfo.cs") && !filename.Contains(".csproj") &&
					!filename.Contains(".ico") && !filename.Contains(".vstemplate"))
					csFileNames.Add(filename);
			}
			foreach (var fileName in csFileNames)
				SourceCodeFiles.Add(Path.Combine(basePath, fileName));
		}

		private readonly IFileSystem fileSystem;
		private readonly string templatePath;

		public string PathToZip { get; private set; }

		private string GetPathToVisualStudioTemplateZip(string templateName)
		{
			var currentPath = GetVstFromCurrentWorkingDirectory();
			if (fileSystem.File.Exists(currentPath))
				return currentPath; //ncrunch: no coverage
			var solutionPath = GetVstFromSolution();
			if (fileSystem.File.Exists(solutionPath))
				return solutionPath;
			var environmentPath = GetVstFromEnvironmentVariable();
			return fileSystem.File.Exists(environmentPath)
				? environmentPath
				: Path.Combine(CsProject.GetVisualStudioProjectsFolder(), "..", "Templates",
					"ProjectTemplates", "Visual C#", "Delta Engine", templateName + ".zip");
		}

		private string GetVstFromCurrentWorkingDirectory()
		{
			return
				Path.GetFullPath(Path.Combine(fileSystem.Directory.GetCurrentDirectory(), templatePath));
		}

		private string GetVstFromSolution()
		{
			return
				Path.GetFullPath(Path.Combine(fileSystem.Directory.GetCurrentDirectory(), "..", "..", "..",
					templatePath));
		}

		private string GetVstFromEnvironmentVariable()
		{
			return Path.Combine(Environment.ExpandEnvironmentVariables("%DeltaEnginePath%"),
				templatePath);
		}

		private string GetBasePath(string fileName)
		{
			return fileName == string.Empty ? "" : fileSystem.Path.GetDirectoryName(fileName);
		}

		public string AssemblyInfo { get; private set; }
		public string Csproj { get; private set; }
		public string Ico { get; private set; }
		public List<string> SourceCodeFiles { get; private set; }

		public static VsTemplate CreateByName(IFileSystem fileSystem, string templateName)
		{
			return new VsTemplate(templateName, new List<string> { "Program.cs", "Game.cs" }, fileSystem);
		}

		public List<string> GetAllFilePathsAsList()
		{
			var list = new List<string> { AssemblyInfo, Csproj, Ico };
			list.AddRange(SourceCodeFiles);
			return list;
		}

		public static string[] GetAllTemplateNames(DeltaEngineFramework framework)
		{
			if (!PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
				return new string[0];
			var templatePath = Path.Combine(PathExtensions.GetDeltaEngineInstalledDirectory(),
				framework.ToString(), "VisualStudioTemplates", "Delta Engine");
			var templateNames = new List<string>();
			foreach (var file in Directory.GetFiles(templatePath))
				templateNames.Add(Path.GetFileNameWithoutExtension(file));
			return templateNames.ToArray();
		}
	}
}