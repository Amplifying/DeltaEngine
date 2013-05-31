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
			PathToZip = GetPathToVisualStudioTemplateZip(templateName, fileSystem);
			var basePath = GetBasePath(PathToZip, fileSystem);
			AssemblyInfo = Path.Combine(basePath, "Properties", "AssemblyInfo.cs");
			Csproj = Path.Combine(basePath, templateName + ".csproj");
			Ico = Path.Combine(basePath, templateName + "Icon.ico");
			SourceCodeFiles = new List<string>();
			foreach (var fileName in sourceCodeFileNames)
				SourceCodeFiles.Add(Path.Combine(basePath, fileName));
		}

		public string PathToZip { get; private set; }

		private static string GetPathToVisualStudioTemplateZip(string templateName,
			IFileSystem fileSystem)
		{
			var solutionPath = GetVstFromCurrentWorkingDirectory(templateName, fileSystem);
			if (fileSystem.File.Exists(solutionPath))
				return solutionPath;

			var environmentPath = GetVstFromEnvironmentVariable(templateName);
			return fileSystem.File.Exists(environmentPath) ? environmentPath : string.Empty;
		}

		private static string GetVstFromCurrentWorkingDirectory(string templateName,
			IFileSystem fileSystem)
		{
			return
				Path.GetFullPath(Path.Combine(fileSystem.Directory.GetCurrentDirectory(), "..", "..", "..",
					VstFolder, "Delta Engine", templateName + ".zip"));
		}

		private const string VstFolder = "VisualStudioTemplates";

		private static string GetVstFromEnvironmentVariable(string templateName)
		{
			return Path.Combine(Environment.ExpandEnvironmentVariables("%DeltaEnginePath%"), VstFolder,
				"Content", templateName + ".zip");
		}

		private static string GetBasePath(string fileName, IFileSystem fileSystem)
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