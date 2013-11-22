using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Required information for creation of a Delta Engine C# project.
	/// </summary>
	public class CsProject
	{
		public CsProject(string userName)
		{
			this.userName = userName.Replace(" ", "");
			starterKit = "EmptyApp";
			SetProjectName();
			Framework = DeltaEngineFramework.OpenTK;
			BaseDirectory = GetVisualStudioProjectsFolder();
		}

		private readonly string userName;
		private string starterKit;

		private void SetProjectName()
		{
			autoProjectName = Name = ConcatenateUserNameAndStarterKit();
		}

		private string autoProjectName;
		public string Name { get; set; }

		private string ConcatenateUserNameAndStarterKit()
		{
			string genitiveS = userName.EndsWith("s", StringComparison.InvariantCultureIgnoreCase)
				? "" : "s";
			return userName + genitiveS + StarterKit;
		}

		public DeltaEngineFramework Framework { get; set; }
		public string BaseDirectory { get; set; }

		public static string GetVisualStudioProjectsFolder()
		{
			foreach (var visualStudioProjectPath in GetSupportedVisualStudioProjectFolders())
				if (Directory.Exists(visualStudioProjectPath))
					return visualStudioProjectPath;
			throw new VisualStudioDefaultProjectsLocationNotFound(); //ncrunch: no coverage
		}

		private static IEnumerable<string> GetSupportedVisualStudioProjectFolders()
		{
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			const string ProjectsFolder = "Projects";
			return new[]
			{
				Path.Combine(myDocumentsPath, "Visual Studio 2013", ProjectsFolder),
				Path.Combine(myDocumentsPath, "Visual Studio 2012", ProjectsFolder),
				Path.Combine(myDocumentsPath, "Visual Studio 2010", ProjectsFolder),
				Path.Combine(myDocumentsPath, "Visual Studio 2008", ProjectsFolder)
			};
		}

		private class VisualStudioDefaultProjectsLocationNotFound : Exception {}

		public string StarterKit
		{
			get { return starterKit; }
			set
			{
				starterKit = value;
				if (IsNameUnchanged())
					SetProjectName();
			}
		}

		public string OutputDirectory
		{
			get { return Path.Combine(BaseDirectory, Name); }
		}

		private bool IsNameUnchanged()
		{
			return autoProjectName == Name;
		}
	}
}