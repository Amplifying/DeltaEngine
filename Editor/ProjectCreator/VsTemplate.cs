using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

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
			PathToZip = GetPathToVisualStudioTemplateZip();
			var basePath = GetBasePath(PathToZip);
			AssemblyInfo = Path.Combine(basePath, "Properties", "AssemblyInfo.cs");
			Csproj = Path.Combine(basePath, templateName + ".csproj");
			Ico = Path.Combine(basePath, templateName + "Icon.ico");
			SourceCodeFiles = new List<string>();
			foreach (var fileName in sourceCodeFileNames)
				SourceCodeFiles.Add(Path.Combine(basePath, fileName));
		}

		private readonly IFileSystem fileSystem;
		private readonly string templatePath;

		public string PathToZip { get; private set; }

		private string GetPathToVisualStudioTemplateZip()
		{
			var currentPath = GetVstFromCurrentWorkingDirectory();
			if (fileSystem.File.Exists(currentPath))
				return currentPath; //ncrunch: no coverage
			var solutionPath = GetVstFromSolution();
			if (fileSystem.File.Exists(solutionPath))
				return solutionPath;
			var environmentPath = GetVstFromEnvironmentVariable();
			return fileSystem.File.Exists(environmentPath) ? environmentPath : string.Empty;
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

		public static VsTemplate GetEmptyGame(IFileSystem fileSystem)
		{
			return new VsTemplate("EmptyGame", new List<string> { "Program.cs", "Game.cs" }, fileSystem);
		}

		public List<string> GetAllFilePathsAsList()
		{
			var list = new List<string> { AssemblyInfo, Csproj, Ico };
			list.AddRange(SourceCodeFiles);
			return list;
		}
	}
}