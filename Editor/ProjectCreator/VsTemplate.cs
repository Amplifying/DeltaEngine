using System.Collections.Generic;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Template ZIP-File from which a new Delta Engine C# project will be created.
	/// </summary>
	public class VsTemplate
	{
		private VsTemplate(string templateName, IEnumerable<string> sourceCodeFileNames)
		{
			PathToZip = "C:\\Code\\DeltaEngine\\VisualStudioTemplates\\Delta Engine\\" + templateName +
				".zip";
			AssemblyInfo = "Properties/AssemblyInfo.cs";
			Csproj = templateName + ".csproj";
			Ico = templateName + "Icon.ico";
			SourceCodeFiles = new List<string>();
			foreach (var fileName in sourceCodeFileNames)
				SourceCodeFiles.Add(fileName);
		}

		public string PathToZip { get; private set; }
		public string AssemblyInfo { get; private set; }
		public string Csproj { get; private set; }
		public string Ico { get; private set; }
		public List<string> SourceCodeFiles { get; private set; }

		public static VsTemplate GetEmptyGame()
		{
			return new VsTemplate("EmptyGame", new List<string> { "Program.cs", "Game.cs" });
		}

		public List<string> GetAllFilePathsAsList()
		{
			var list = new List<string> { AssemblyInfo, Csproj, Ico };
			list.AddRange(SourceCodeFiles);
			return list;
		}
	}
}